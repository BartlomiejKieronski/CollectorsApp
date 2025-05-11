import "./PasswordInput.css"
import { useState } from "react";
export default function InputPassword({ setPassword }) {
    const [showPassword, setShowPassword] = useState();

    const handleTouchStart = () => setShowPassword(true);
    const handleTouchEnd = () => setShowPassword(false);

    const handleMouseDown = () => setShowPassword(true);
    const handleMouseUp = () => setShowPassword(false);
    const handleMouseLeave = () => setShowPassword(false);

    return (
        <div className="password-container">
            <input onChange={setPassword} placeholder="Hasło" className="input-width input-height, input-style" name="password" type={showPassword ? "text" : "password"} defaultValue={"666666666"} required />
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
    )
}