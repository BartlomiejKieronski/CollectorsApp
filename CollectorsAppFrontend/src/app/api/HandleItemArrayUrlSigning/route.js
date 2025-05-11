import { Storage } from "@google-cloud/storage";
const bucketName = "collector-app-2025";

const storage = new Storage({ keyFilename: "C:/AuthGCS.json" });

export async function POST(req, res) {
  if (req.method !== "POST") {
    return Response.json({ error: "Method Not Allowed" });
  }

  try {
    const items = await req.json();

    if (!Array.isArray(items) || items.length == 0) {
      return Response.json({ error: "No files provided" });
    }

    const generateSignedUrl = async (filePath) => {
      const options = {
        version: "v4",
        action: "read",
        expires: Date.now() + 2 * 60 * 60 * 1000,
      };

      const [url] = await storage
        .bucket(bucketName)
        .file(filePath)
        .getSignedUrl(options);

      return url;
    };

    const signedUrlsPromises = items.map((item) => {

      if (item.photoFilePath != null) {
        return generateSignedUrl(item.photoFilePath)
      } else {
        return "/placeholder_image.png"
      }
    });

    const signedUrls = await Promise.all(signedUrlsPromises);

    const responseData = items.map((item, index) => ({
      ...item,
      url: signedUrls[index],
    }));

    return Response.json({ responseData });
  } catch (ex) {
    return Response.json({
      message: "Error generating signed URLs",
      error: ex.message,
    });
  }
}