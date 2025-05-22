"use client";

import Image from "next/image";
import Style from "../ResetPassword.module.css";
import { useParams, useRouter } from "next/navigation";
import axios from "axios";
import { useEffect, useState } from "react";
import jwt from "jsonwebtoken"
import Button from "@/app/Components/Button/Button";
import cn from "classnames";

export default function ResetPassword() {
    const params = useParams();
    const router = useRouter();
    const { Reset } = params;

    const [error, setError] = useState();
    const [showPassword, setShowPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [isDisabled, setIsDisabled] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        console.log(name)
        console.log(value)
    }
    useEffect(() => {
        if (Reset) {
            const isValidJWT = (token) => {
                try {
                    const decoded = jwt.decode(token, { complete: true });
                    return !!decoded;
                } catch (error) {
                    return false;
                }
            };
            if (isValidJWT(Reset)) {

            }
            else {
                //router.push("/Login")
            }
        }
    }, [Reset])
    const handleResetPassword = async (e) => {
        e.preventDefault();

        if (Reset) {
            const password = e.target.password.value;
            const repeatPassword = e.target.repeatPassword.value;
            if (password == repeatPassword) {
                setIsLoading(true);
                await axios.post("api/Authentication/PwdReset", { password: password, token: Reset }, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': "bearer " + Reset
                    }
                }).then(res => {
                    if (res.status == 200) {
                        setIsLoading(false);
                        alert("Wiadomość email z linkiem do zmieny hasła został wysłany na adres email. W przypadku braku wiadomości sprawdź folder ze spamem")
                    }
                    else {
                        setIsLoading(false);
                        alert("Coś poszło nie tak")
                    }
                })
            } else {
                setError("Hasła się nie zgadzają")
            }
        }
        else {
            alert("Brak poprawnego tokenu")
        }
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
                        Hasło:
                        <input onChange={handleChange}
                            id="password"
                            name="password"
                            type="password"
                            required
                            placeholder="Wpisz hasło"
                            className={cn(Style.input)}
                        />
                    </label>
                    <label className={cn(Style.label)}>
                        Powtórz hasło:s
                        <input
                            id="repeatPassword"
                            name="repeatpassword"
                            type="password"
                            required
                            placeholder="Powtórz hasło"
                            className={cn(Style.input)}
                        />
                    </label>
                    <Button type="submit" required={true} disabled={isDisabled} isLoading={isLoading}>
                        Zresetuj hasło
                    </Button>
                </form>
            </div>
        </div>
    );
}