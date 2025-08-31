"use client"
import Style from"./AccountStyle.module.css"
import { useState } from "react"
import { useMenuItemsProvider } from "@/app/Providers/MenuProvider/MenuProvider";
import { useSession } from "next-auth/react";
import { useRouter } from "next/navigation";
import { isPasswordCorrect, deleteUserAccount } from "@/app/lib/authUtils";
import { deleteCollection } from "@/app/lib/collectionsUtils";
import { toast } from "react-toastify";
import Button from "@/app/Components/Button/Button";
import InputPassword from "@/app/Components/PasswordInput/PasswordInput";
import cn from "classnames" 

export default function AccountSettings() {
    const { data: session, status } = useSession();
    const { menuItems, areItemsLoading } = useMenuItemsProvider();

    const [isDeleting, setIsDeleting] = useState(false);
    const [passwordForDelete, setPasswordForDelete] = useState(null);
    const [passwordForChange, setPasswordForChange] = useState(null);
    const [isLoading, setIsLoading] = useState();

    const router = useRouter();

    const deleteAccount = async () => {
        menuItems.forEach(async (collection) => {
            await deleteCollection(collection);
        });
        await deleteUserAccount(session.user.id)
        router.push("/Logout")
    }

    const changePassword = async () => {
        await instance.post("api/Authentication/PwdReset", { email: user.session.email }).then(res => {
            toast(`Link do zmiany hasła został wysłany na adres email przypisany do tego konta : ${session.user.email}`, { autoClose: false })
        })
    }

    const checkPassword = async (type, password) => {
        setIsLoading(true);
        if (password == null) {
            setIsLoading(false)
            toast("Najpierw wprowadź hasło")
            return;
        }
        var result = await isPasswordCorrect({
            "name": session.user.email,
            "password": password
        });
        if (type == "change" && result.data == true) {
            await changePassword();
        }
        else if (type == "delete" && result.data == true) {
            await deleteAccount();
        }
        else {
            toast("Błędne hasło", { autoClosefalse });
        }
        setIsLoading(false)
    }

    const handlePasswordChange = (e) => setPasswordForChange(e.target.value);
    const handleAccountDelete = (e) => setPasswordForDelete(e.target.value);

    if (status == "authenticated" && areItemsLoading == false) {
        return (
            <div className={cn(Style.accountContentContainer)}>
                <div className={cn(Style.passwordChange)}>
                    <div>Zmień hasło:</div>
                    <div>
                        <label>Wprowadź obecne hasło: &nbsp;
                            <div><InputPassword setPassword={handlePasswordChange} /></div>
                        </label>
                        <div>
                            <Button isLoading={isLoading} disabled={isLoading} type="button" onClick={() => checkPassword("change", passwordForChange)}>Zmień hasło</Button>
                        </div>
                    </div>
                </div>
                <div className={cn(Style.deleteAccount)}>
                    {!isDeleting && <div className={cn(Style.deleteButton)}>
                        <div>Usuń Konto</div>
                        <div><button onClick={() => setIsDeleting(true)}>Usuń konto</button></div>
                    </div>}
                    {isDeleting && <div className={cn(Style.deleteButtonConfirm)}>
                        <div>
                            <div>Podaj hasło</div>
                            <div><InputPassword setPassword={handleAccountDelete} /></div>
                        </div>
                        <div>
                            <button onClick={() => setIsDeleting(false)}>Anuluj</button>
                            <Button isLoading={isLoading} disabled={isLoading} type="button" onClick={() => checkPassword("delete", passwordForDelete)}>Usuń</Button>
                        </div>
                    </div>}
                </div>
            </div>
        )
    }
}