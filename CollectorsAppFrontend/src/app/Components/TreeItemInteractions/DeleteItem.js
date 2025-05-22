"use client"
import Button from "../Button/Button"
import Style from "../Tree/SideMenuTreeComponent.module.css";
import cn from "classnames";

export default function DeleteTreeItem({DeleteItem, SelectedParent,isLoading,SetSelectedItem}) {

    return (
        <div className={cn(Style.editItemContainer)}>
            <div><p>Czy na pewno chcesz usunąć element?</p></div>
            <div className={cn(Style.addItemButton)}>
                <Button onClick={()=>DeleteItem(SelectedParent.id)} isLoading={isLoading} disabled={isLoading} type={"button"}>Potwierdź</Button>
                <button onClick={()=>SetSelectedItem(null)}>Anuluj</button>
            </div>
        </div>
    )

}