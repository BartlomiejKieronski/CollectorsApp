"use client";

import { useState, useEffect, useRef } from "react";
import Image from "next/image";
import "./Carousel.css";
import rightArrow from "@/../public/right_arrow.svg";
import leftArrow from "@/../public/left_arrow.svg";

export default function ImageDisplay({ signedUrlImageData }) {
  const [current, setCurrent] = useState(0);
  const [prev, setPrev] = useState(null);
  const [direction, setDirection] = useState("next");
  const [lastInteraction, setLastInteraction] = useState(Date.now());
  const isFirstRender = useRef(true);

  const touchStartX = useRef(null);
  const touchEndX = useRef(null);

  const [arr, setArr] = useState(null);

  useEffect(() => {
    isFirstRender.current = false;
  }, []);

  useEffect(() => {
    setArr(signedUrlImageData)
  }, [signedUrlImageData])

  useEffect(() => {
    if (!arr) return;
    const preloadRange = 1; 
    const toPreload = new Set();
    for (let offset = -preloadRange; offset <= preloadRange; offset++) {
      const index = (current + offset + arr.length) % arr.length;
      toPreload.add(arr[index].url);
    }
    toPreload.forEach((url) => {
      const img = new window.Image();
      img.src = url;
    });
  }, [current, arr]);

  function clearPrevAfterAnimation() {
    setTimeout(() => setPrev(null), 500);
  }

  function nextImage() {
    setDirection("next");
    setCurrent((prevCurrent) => {
      setPrev(prevCurrent);
      const newIndex = prevCurrent < arr.length - 1 ? prevCurrent + 1 : 0;
      return newIndex;
    });
    setLastInteraction(Date.now());
    clearPrevAfterAnimation();
  }

  function prevImage() {
    setDirection("prev");
    setCurrent((prevCurrent) => {
      setPrev(prevCurrent);
      const newIndex = prevCurrent > 0 ? prevCurrent - 1 : arr.length - 1;
      return newIndex;
    });
    setLastInteraction(Date.now());
    clearPrevAfterAnimation();
  }

  function jumpTo(index) {
    if (index !== current) {
      setDirection(index > current ? "next" : "prev");
      setPrev(current);
      setCurrent(index);
      clearPrevAfterAnimation();
    }
    setLastInteraction(Date.now());
  }

  useEffect(() => {
    if (!arr || arr.length<2) return;

    const autoSwitchInterval = setInterval(() => {
      setCurrent((prevCurrent) => {
        setDirection("next");
        setPrev(prevCurrent);
        const newIndex = prevCurrent < arr.length - 1 ? prevCurrent + 1 : 0;
        return newIndex;
      });
      clearPrevAfterAnimation();
    }, 15000);

    return () => clearInterval(autoSwitchInterval);
  }, [lastInteraction, arr]);

  const handleTouchStart=(e)=> touchStartX.current = e.touches[0].clientX;

  const handleTouchMove=(e)=> touchEndX.current = e.touches[0].clientX;

  const handleTouchEnd=()=> {
    if (touchStartX.current === null || touchEndX.current === null || arr.length <2) return;
    const distance = touchStartX.current - touchEndX.current;

    if (distance > 50) {
      nextImage();
    } else if (distance < -50) {
      prevImage();
    }

    touchStartX.current = null;
    touchEndX.current = null;
  }

  const newImageClass = isFirstRender.current
    ? ""
    : direction === "next"
      ? "slideInNext"
      : "slideInPrev";

  const prevImageClass =
    direction === "next" ? "slideOutNext" : "slideOutPrev";

  return (
    <div className="carousel-container">
      {arr && (<>
        <div
          className="carousel"
          onTouchStart={handleTouchStart}
          onTouchMove={handleTouchMove}
          onTouchEnd={handleTouchEnd}
        >
          {Array.isArray(arr) && arr.length > 1 && (
            <button className="nav-button left-button" onClick={prevImage}>
              <Image src={leftArrow} fill alt="loading" sizes="100%" />
            </button>
          )}
          <div className="image-wrapper">
            {prev !== null && Array.isArray(arr) && arr.length>1&&(
              <Image
                key={`prev-${arr[prev].id}`}
                className={`carousel-image ${prevImageClass}`}
                alt="Previous"
                sizes="100%"
                src={arr[prev].url}
                fill
              />
            )}
            <Image
              key={`current-${arr[current].id}`}
              className={`carousel-image ${newImageClass}`}
              alt="Current"
              src={arr[current].url}
              sizes="100%"
              fill />
          </div>
          {Array.isArray(arr) && arr.length > 1 && (
            <button className="nav-button right-button" onClick={nextImage}>
              <Image src={rightArrow} fill alt="loading" />
            </button>
          )}
        </div>{Array.isArray(arr) && arr.length>0 &&(<>
        <hr />
        <div className="indicators-wrapper">
          <div className="indicators">
            {arr.map((x, index) => (
              <div
                className={`indicator ${index === current ? "active" : ""}`}
                key={x.id} onClick={() => jumpTo(index)}>
                <Image src={x.url} width={80} height={60} alt="loading" sizes="100%" />
              </div>
            ))}
          </div>
        </div>
        <hr />
      </>)}
      </>)}
    </div>
    
  );
}
