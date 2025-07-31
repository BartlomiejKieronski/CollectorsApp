"use client";

import { useEffect, useState} from "react";
import { useSession, getCsrfToken, signIn } from "next-auth/react";
import { useRouter, useParams } from "next/navigation";
import Style from "./styles.module.css"
import Image from "next/image";
import Head from "next/head";
import Button from "@/app/Components/Button/Button";
import Link from "next/link";
import cn from "classnames";

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
    if(segments=="SessionError"){
      alert("Wystąpił problem z sesją użytkownia. Proszę zalogować się ponownie!");
    }
    else if(segments=="Logged-Out"){
      alert("Wylogowano pomyślnie");
    }
    else if(segments!=null){
      alert(segments)
    }
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
        callbackUrl: "/ViewItems",
      });

      if (result.ok) {
        setLoading(false);
        setIsDisabled(false);
        router.push("/ViewItems")
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
      <div className={cn(Style.displayBlock)}>
        {!session ?
          (<div className={cn(Style.divCenter)}>
            <div className={cn(Style.borderLogin, Style.paddingBorder, Style.borderRadius, Style.paddingBottom, Style.paddingTop)}>
            <div className={cn(Style.logoDisplay)}>
              <Image
              src="/android-chrome-512x512.png"
              alt="Logo"
              width={100}
              height={100}
            /></div>
              <div className={cn(Style.displayCenter)}><h2>Logowanie</h2></div>
              <form onSubmit={(e) => handleSubmit(e)}>
                <input name="csrfToken" type="hidden" defaultValue={csrfToken} />
                <div>
                  <label> Nazwa użytkownika lub Email: <br />
                    <input className={cn(Style.inputWidth, Style.inputHeight, Style.inputStyle)} name="username" type="text" required />
                  </label>
                </div>
                <br />
                <label >
                  Hasło:<br />
                  <div className={cn(Style.passwordContainer)}>
                    <input className={cn(Style.inputWidth, Style.inputHeight, Style.inputStyle)} name="password" type={showPassword ? "text" : "password"} required />
                    <button
                      type="button"
                      onTouchStart={handleTouchStart}
                      onTouchEnd={handleTouchEnd}
                      onMouseDown={handleMouseDown}
                      onMouseUp={handleMouseUp}
                      onMouseLeave={handleMouseLeave}
                      className={cn(Style.toggleasswordIcon)}
                    >
                      {showPassword ? "👁️‍🗨️" : "👁️"}
                    </button>
                  </div>
                </label>
                <br />
                <div className={cn(Style.buttonMargin)}>
                  <Button classes={cn(Style.button)} type="submit" required={true} disabled={isDisabled} isLoading={loading}>Zaloguj się</Button>
                </div>
                <p>{error}</p>
                <br />
                <div className={cn(Style.cardLinks)}>
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