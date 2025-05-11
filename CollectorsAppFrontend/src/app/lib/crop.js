export function cropImage(image, crop) {
  if (!image || !crop?.width || !crop?.height) return null;

  const pixelCrop = {
    x: (crop.x / 100) * image.naturalWidth,
    y: (crop.y / 100) * image.naturalHeight,
    width: (crop.width / 100) * image.naturalWidth,
    height: (crop.height / 100) * image.naturalHeight,
  };

  const canvas = document.createElement("canvas");
  canvas.width = pixelCrop.width;
  canvas.height = pixelCrop.height;
  const ctx = canvas.getContext("2d");

  ctx.drawImage(
    image,
    pixelCrop.x,
    pixelCrop.y,
    pixelCrop.width,
    pixelCrop.height,
    0,
    0,
    pixelCrop.width,
    pixelCrop.height
  );

  return canvas.toDataURL("image/jpeg");
}