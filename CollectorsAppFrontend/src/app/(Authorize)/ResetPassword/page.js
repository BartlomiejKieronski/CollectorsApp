"use client";

import Image from "next/image";
import Style from"./ResetPassword.module.css"
import instance from "@/app/axiosInstance";
import { useState } from "react";
import Button from "@/app/Components/Button/Button";
import cn from "classnames"
export default function ResetPassword() {
  const [isLoading, setIsLoading] = useState(false);
  const [isDisabled, setIsDisabled] = useState(false);

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    const email = e.target.email.value;
    await instance.post("api/Authentication/PwdReset", { email: email }).then(res => {
      setIsLoading(false);
      if (res.status == 200) {
        alert("Wiadomość email z linkiem do zmieny hasła został wysłany na adres email. W przypadku braku wiadomości sprawdź folder ze spamem");
      } else {
        alert("Użytkownik z danym adresem email nie został odnaleziony");
      }
    })
  };

  return (
    <div className={cn(Style.container)}>
      <div className={cn(Style.card)}>
        <div className={cn(Style.logo)}>
          <Image
            src="/android-chrome-512x512.png"
            alt="Logo"
            width={100}
            height={100}
          />
        </div>
        <h2 className={cn(Style.title)}>Resetowanie hasła</h2>
        <form onSubmit={handleResetPassword} className={cn(Style.form)}>
          <label className={cn(Style.label)}>
            Adres E-mail
          
          <input
            id="email"
            name="email"
            type="email"
            required
            placeholder="Podaj email"
            className={cn(Style.input)}
          /></label>
          <Button  type="submit" required={true} disabled={isDisabled} isLoading={isLoading}>
            Resetuj hasło
          </Button>
        </form>
      </div>
    </div>
  );
}