"use client"
import Button from "../Button/Button"

export default function DeleteTreeItem({DeleteItem, SelectedParent,isLoading,SetSelectedItem}) {

    return (
        <div className="edit-item-container">
            <div><p>Czy na pewno chcesz usunąć element?</p></div>
            <div className="add-item-button">
                <Button onClick={()=>DeleteItem(SelectedParent.id)} isLoading={isLoading} disabled={isLoading} type={"button"}>Potwierdź</Button>
                <button onClick={()=>SetSelectedItem(null)}>Anuluj</button>
            </div>
        </div>
    )

}