"use client";

import { useState, useRef } from "react";
import "react-image-crop/dist/ReactCrop.css";
import ReactCrop, { centerCrop, makeAspectCrop } from "react-image-crop";
import "./ImageCroper.css";

const ASPECT_RATIO = 16 / 12;
const MIN_DIMENSION = 350;

export default function ImageCroper({ onCroppedImageChange }) {
  const [imgSrc, setImgSrc] = useState("");
  const [crop, setCrop] = useState(null);
  const [croppedImage, setCroppedImage] = useState(null);
  const [imgUploadError, setImageUploadError] = useState(null);
  const [minCropWidth, setMinCropWidth] = useState(null);

  const imgRef = useRef(null);

  const onSelectFile = (e) => {
    setCroppedImage(null);
    setCrop(null);
    setImageUploadError(null);
    
    const file = e.target.files?.[0];
    if (!file) return;
    if (file.size < 1) {
      setImageUploadError("Plik nie zawiera zawartości.");
      return;
    }

    const fileParts = file.name.split(".");
    const ext = fileParts[fileParts.length - 1].toLowerCase();

    if (ext !== "jpg" && ext !== "png" && ext !== "jpeg") {
      setImageUploadError(
        `Nieprawidłowy format pliku: .${ext}. Akceptowalne formaty to: .jpg, .jpeg, .png.`
      );
      setImgSrc("");
      setCroppedImage(null);
      onCroppedImageChange(null);
      return;
    }

    const reader = new FileReader();
    reader.addEventListener("load", () => {
      const imageUrl = reader.result?.toString() || "";
      setImgSrc(imageUrl);
    });
    reader.readAsDataURL(file);
  };

  const onImageLoad = (e) => {
    const image = e.currentTarget;
    imgRef.current = image;

    const computedMinWidth = image.naturalWidth < MIN_DIMENSION ? image.naturalWidth : MIN_DIMENSION;

    const displayedImageWidth = image.width;
    const displayedImageHeight = image.height;

    const displayedMinWidth = computedMinWidth * (displayedImageWidth / image.naturalWidth);
    setMinCropWidth(displayedMinWidth);

    const cropWidthPercent = (computedMinWidth / image.naturalWidth) * 100;

    const initialCrop = makeAspectCrop(
      {
        unit: "%",
        width: cropWidthPercent,
      },
      ASPECT_RATIO,
      displayedImageWidth,
      displayedImageHeight
    );
    const centeredCrop = centerCrop(initialCrop, displayedImageWidth, displayedImageHeight);
    
    setCrop(centeredCrop);
  };

  const handleCrop = (completedCrop) => {
    if (!imgRef.current || !completedCrop?.width || !completedCrop?.height) return;
    
    const image = imgRef.current;
    const scaleX = image.naturalWidth / image.width;
    const scaleY = image.naturalHeight / image.height;
    
    const canvas = document.createElement("canvas");
    canvas.width = completedCrop.width * scaleX;
    canvas.height = completedCrop.height * scaleY;
    const ctx = canvas.getContext("2d");

    ctx.drawImage(
      image,
      completedCrop.x * scaleX,
      completedCrop.y * scaleY,
      completedCrop.width * scaleX,
      completedCrop.height * scaleY,
      0,
      0,
      completedCrop.width * scaleX,
      completedCrop.height * scaleY
    );

    const croppedDataUrl = canvas.toDataURL("image/jpeg");
    setCroppedImage(croppedDataUrl);
    if (onCroppedImageChange) {
      onCroppedImageChange(croppedDataUrl);
    }
  };

  return (
    <div>
      <div>
        <label>
          Wybierz zdjęcie
          <input type="file" onChange={onSelectFile} />
        </label>
      </div>
      <div style={{ paddingLeft: "4px", paddingTop: "1vh" }}>
        {imgUploadError}
      </div>
      {imgSrc && (
        <div className="flex flex-col items-center react-crop-max-width">
          <ReactCrop
            crop={crop}
            onChange={(newCrop) => setCrop(newCrop)}
            onComplete={(c) => handleCrop(c)}
            keepSelection
            aspect={ASPECT_RATIO}
            // Use the displayed minimum width so the crop adjusts correctly
            minWidth={minCropWidth}
          >
            <img
              src={imgSrc}
              alt="upload"
              className="max-width"
              onLoad={onImageLoad}
              ref={imgRef}
            />
          </ReactCrop>
          <br />
          <p className="preview">Podgląd:</p>
        </div>
      )}
      {croppedImage && (
        <img
          src={croppedImage}
          className="max-width"
          alt="Cropped image preview"
        />
      )}
    </div>
  );
}
