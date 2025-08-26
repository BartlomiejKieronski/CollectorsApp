import { Storage } from "@google-cloud/storage";

const bucketName = 'collector-app-2025';
const storage = new Storage();
const bucket = storage.bucket(bucketName);

export async function DELETE(req, { params }) {
    if (req.method !== "DELETE") {
        return Response.json({ error: "Method Not Allowed" });
      }

    const { path } = await params  
    const gcsPath = decodeURIComponent(path)
    
    if (!gcsPath) {
        return Response.json({ error: "No file path provided" })
    }
    
    try {
        bucket.file(gcsPath).delete();
        return Response.json({message: "File deleted", file: gcsPath })
    }
    catch(ex){
        return Response.json({error:er})
    }
}