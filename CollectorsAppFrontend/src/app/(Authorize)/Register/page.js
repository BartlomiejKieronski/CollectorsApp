"use client"
import Style from "./cssRegister.module.css"
import { useRouter } from "next/navigation"
import { useState } from "react"
import instance from "@/app/axiosInstance"
import Head from "next/head"
import Image from "next/image"
import Button from "@/app/Components/Button/Button"
import Link from "next/link"
import cn from "classnames"

export default function Register() {
    const [axiosResponse, setAxiosResponse] = useState();
    const [error, setError] = useState();
    const [showPassword, setShowPassword] = useState(false);
    const [showRepeatPassword, setShowRepeatPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [isDisabled, setIsDisabled] = useState(false)
    const router = useRouter();

    const handleTouchStart = () => setShowPassword(true);
    const handleTouchEnd = () => setShowPassword(false);
    const handleMouseDown = () => setShowPassword(true);
    const handleMouseUp = () => setShowPassword(false);
    const handleMouseLeave = () => setShowPassword(false);

    const handleTouchStartRepeat = () => setShowRepeatPassword(true);
    const handleTouchEndRepeat = () => setShowRepeatPassword(false);
    const handleMouseDownRepeat = () => setShowRepeatPassword(true);
    const handleMouseUpRepeat = () => setShowRepeatPassword(false);
    const handleMouseLeaveRepeat = () => setShowRepeatPassword(false);


    async function RegisterUser(e) {
        e.preventDefault()
        setIsLoading(true);
        if (e.target.registerPassword.value !== e.target.repeatPassword.value) {
            setError("Hasła się nie zgadzają!")
            setIsLoading(false);
        } else if (e.target.registerPassword.value.length < 8) {
            setError("Hasło musi mieć co najmniej 8 znaków!")
            setIsLoading(false);
        } else {
            const result = await instance.post("api/Users", {
                name: e.target.registerName.value,
                email: e.target.registerEmail.value,
                password: e.target.registerPassword.value,
            }).then(response => {
                setAxiosResponse(response.data);
            }).then(() => {
                setIsLoading(false);
                if (axiosResponse === "Created sucesfully") {
                    router.push("/Login")
                }
            })
        }
    }

    return (
        <>
            <Head>
                <title>Rejestracja</title>
            </Head>
            <div className={cn(Style.displayBlock)}>
                <div className={cn(Style.divCenter)}>
                    <div className={cn(Style.divBorder)}>
                        <div className={cn(Style.logoDisplay)}>
                            <Image
                                src="/android-chrome-512x512.png"
                                alt="Logo"
                                width={100}
                                height={100}
                            /></div>
                        <div className={cn(Style.textCenter)}>
                            <h2>Zarejestruj się</h2>
                        </div>
                        <div>
                            <form onSubmit={(e) => RegisterUser(e)}>
                                <br />
                                <label>
                                    Nazwa użytkownika:
                                    <input className={cn(Style.inputBorder, Style.inputWidth, Style.inputHeight)} name="registerName" type="text" required />
                                </label>
                                <br />
                                <label>
                                    Email:
                                    <input className={cn(Style.inputBorder, Style.inputWidth, Style.inputHeight)} name="registerEmail" type="text" required />
                                </label>
                                <br />
                                <label>
                                    Hasło:
                                    <div className={cn(Style.passwordContainer)}>
                                        <input
                                            className={cn(Style.inputBorder, Style.inputWidth, Style.inputHeight)}
                                            name="registerPassword"
                                            type={showPassword ? "text" : "password"}
                                            required
                                        />
                                        <button
                                            type="button"
                                            onTouchStart={handleTouchStart}
                                            onTouchEnd={handleTouchEnd}
                                            onMouseDown={handleMouseDown}
                                            onMouseUp={handleMouseUp}
                                            onMouseLeave={handleMouseLeave}
                                            className={cn(Style.togglePasswordIcon)}
                                        >
                                            {showPassword ? "👁️‍🗨️" : "👁️"}
                                        </button>
                                    </div>
                                </label>
                                <br />
                                <label className={cn(Style.passwordLabel)}>
                                    Powtórz hasło:
                                    <div className={cn(Style.passwordContainer)}>
                                        <input
                                            className={cn(Style.inputBorder, Style.inputWidth, Style.inputHeight)}
                                            name="repeatPassword"
                                            type={showRepeatPassword ? "text" : "password"}
                                            required
                                        />
                                        <button
                                            type={cn(Style.button)}
                                            onTouchStart={handleTouchStartRepeat}
                                            onTouchEnd={handleTouchEndRepeat}
                                            onMouseDown={handleMouseDownRepeat}
                                            onMouseUp={handleMouseUpRepeat}
                                            onMouseLeave={handleMouseLeaveRepeat}
                                            className={cn(Style.togglePasswordIcon)}
                                        >
                                            {showRepeatPassword ? "👁️‍🗨️" : "👁️"}
                                        </button>
                                    </div>
                                </label>
                                {error && (
                                    <>
                                        <br />
                                        <p className={cn(Style.errorMessage)}>{error}</p>
                                    </>
                                )}
                                <br />
                                {axiosResponse}
                                <Button classes={cn(Style.SBS)} type="submit" required={true} disabled={isDisabled} isLoading={isLoading}>Zarejestruj się</Button>
                            </form>
                            <br />
                            <div className={cn(Style.cardLinks)}>
                                <div><Link href={"/Login"}>Zaloguj się</Link></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    )
}