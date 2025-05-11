import { Storage } from "@google-cloud/storage";
import path from "path";

export const config = {
    api: {
      bodyParser: {
        sizeLimit: '4mb',
      },
    },
  };

const storage = new Storage();

const bucketName = 'collector-app-2025'
const bucket = storage.bucket(bucketName);

export async function POST(req, res) {
    
    if (req.method !== "POST") {
        return Response.json({ error: "Method Not Allowed" });
    }

    try {
        const body = await req.json();
        const { fileName, fileData } = body;
        if (!fileName || !fileData) {
            return Response.json({ error: "Missing file name or data" });
        }

        const buffer = Buffer.from(fileData, "base64");

        const file = bucket.file(fileName);

        await file.save(buffer, {
            metadata: { contentType: getContentType(fileName) },
        });

        return Response.json({
            message: "File uploaded successfully",
            url: `https://storage.googleapis.com/${bucketName}/${fileName}`,
        });
    } 
    catch (error) {
        console.error("Upload Error:", error);
        return Response.json({ error: "Internal Server Error" });
    }
}

function getContentType(fileName) {
    const ext = path.extname(fileName).toLowerCase();
    const mimeTypes = {
        ".png": "image/png",
        ".jpg": "image/jpeg",
        ".jpeg": "image/jpeg"
    };
    return mimeTypes[ext] || "application/octet-stream";
}
