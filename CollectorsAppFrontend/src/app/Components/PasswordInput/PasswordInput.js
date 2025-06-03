import Style from "./PasswordInput.module.css";
import cn from "classnames";
import { useState } from "react";

export default function InputPassword({ setPassword }) {
    const [showPassword, setShowPassword] = useState();

    const handleTouchStart = () => setShowPassword(true);
    const handleTouchEnd = () => setShowPassword(false);

    const handleMouseDown = () => setShowPassword(true);
    const handleMouseUp = () => setShowPassword(false);
    const handleMouseLeave = () => setShowPassword(false);

    return (
        <div className={cn(Style.passwordContainer)}>
            <input onChange={setPassword} placeholder="Hasło" className={cn(Style.inputWidth, Style.inputHeight, Style.inputStyle )} name="password" type={showPassword ? "text" : "password"} required />
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
    )
}