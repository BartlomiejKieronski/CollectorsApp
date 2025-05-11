import { Storage } from "@google-cloud/storage";
const bucketName = 'collector-app-2025';

const storage = new Storage(
    { keyFilename: 'C:/AuthGCS.json' }
);

export async function POST(req, res) {
    if (req.method !== "POST") {
        return Response.json({ error: "Method Not Allowed" });
    }
    var imgUrl= ""

    try {
        const body = await req.json();
        console.log(body)
        const { fileName } = body;
        async function generateV4ReadSignedUrl() {
            const options = {
                version: 'v4',
                action: 'read',
                expires: Date.now() + 60 * 60 * 1000,
            };
        
        const [url] = await storage
            .bucket(bucketName)
            .file(fileName)
            .getSignedUrl(options);
        
        imgUrl = url
        }
        var l = await generateV4ReadSignedUrl().catch(console.error);

        return Response.json({message:"Generated GET signed URL",url:imgUrl,response:l,curl:`curl '${imgUrl}'`})
} catch (ex) {
        return Response.json({ message: `ex` })
    }
}