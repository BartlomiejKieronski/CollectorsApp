"use client";

import { useEffect, useState,useMemo } from "react";
import { useSession, getCsrfToken, signIn, signOut } from "next-auth/react";
import { useRouter, useParams } from "next/navigation";
import "./styles.css"
import Image from "next/image";
import Head from "next/head";
import Button from "@/app/Components/Button/Button";
import Link from "next/link";

export default function SignInPage({ csrfToken }) {
  const { data: session, status } = useSession();
  const [error, setError] = useState(null);
  const [showPassword, setShowPassword] = useState(false);
  const router = useRouter();
  const [isDisabled, setIsDisabled] = useState(false);
  const [loading,setLoading] = useState(false);
  const params = useParams();

  const segments = params.Info;
  
  useEffect(()=>{
    console.log(segments)
  },[segments])
  
  const handleTouchStart = () => setShowPassword(true);
  const handleTouchEnd = () => setShowPassword(false);
  const handleMouseDown = () => setShowPassword(true);
  const handleMouseUp = () => setShowPassword(false);
  const handleMouseLeave = () => setShowPassword(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setIsDisabled(true);
    setLoading(true);
    setError(null);

    const username = e.target.username.value;
    const password = e.target.password.value;

    try {
      const result = await signIn("credentials", {
        redirect: false,
        username,
        password,
        callbackUrl: "/",
      });

      if (result.ok) {
        setLoading(false);
        setIsDisabled(false);
        router.push("/")
      }
      if (result.status === 401) {
        setLoading(false);
        setIsDisabled(false);
        setError("Nieprawidłowa nazwa użytkownika lub hasło.");
        return;
      }

    } catch (err) {
      setError("Coś poszło nie tak, spróbuj ponownie");
    }
  };

  if (status === "loading") {
    return;
  }
  
  else {
    return (<div>
      <Head>
        <title>Logowanie</title>
      </Head>
      <div className="display-block">
        {!session ?
          (<div className="div-center">
            <div className="border-login padding-border border-radius padding-bottom padding-top bcg-clr">
            <div className="l-d-display">
              <Image
              src="/android-chrome-512x512.png"
              alt="Logo"
              width={100}
              height={100}
            /></div>
              <div className="l-d-display"><h2>Logowanie</h2></div>
              <form onSubmit={(e) => handleSubmit(e)}>
                <input name="csrfToken" type="hidden" defaultValue={csrfToken} />
                <div className="">
                  <label> Nazwa użytkownika lub Email: <br />
                    <input className="input-width input-height input-style" name="username" type="text" defaultValue={"Bartek"} required />
                  </label>
                </div>
                <br />
                <label >
                  Hasło:<br />
                  <div className="password-container">
                    <input className="input-width input-height input-style" name="password" type={showPassword ? "text" : "password"} defaultValue={"666666666"} required />
                    <button
                      type="button"
                      onTouchStart={handleTouchStart}
                      onTouchEnd={handleTouchEnd}
                      onMouseDown={handleMouseDown}
                      onMouseUp={handleMouseUp}
                      onMouseLeave={handleMouseLeave}
                      className="toggle-password-icon"
                    >
                      {showPassword ? "👁️‍🗨️" : "👁️"}
                    </button>
                  </div>
                </label>
                <br />
                <div>
                  <Button classes="button" type="submit" required={true} disabled={isDisabled} isLoading={loading}>Zaloguj się</Button>
                </div>
                <p>{error}</p>
                <br />
                <div className="card-links">
                  <div><Link href={"/Register"}>Zarejestruj się</Link></div>
                  <div><Link href={"/ResetPassword"}>Zapomniałem hasło</Link></div>

                </div>
              </form>
            </div>
          </div>) : (
            <div>
            </div>
          )}
      </div>
    </div>
    )
  }
}

export async function getServerSidePropsname(context) {
  const csrfToken = await getCsrfToken(context);
  return {
    props: { csrfToken },
  };
}