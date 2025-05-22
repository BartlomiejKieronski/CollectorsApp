import Button from "../Button/Button"
import Style from "../Tree/SideMenuTreeComponent.module.css";
import cn from "classnames";

export default function EditTreeItem({ isLoading, isDisabled, classes, changeName, setName, PreviusName, name, SetSelectedParent }) {

    return (
        <div className={cn(Style.editItemContainer)}>
            <div>
                <label>
                    <p>Poprzednia nazwa:</p>
                    {PreviusName.name}
                    <input type="text" name="changeItem" defaultValue={PreviusName.name} placeholder="Nowa nazwa" id="changeName" onChange={(e) => setName(e.target.value)} />
                </label>
            </div>
            <div className={cn(Style.addItemButton)}>
                <Button classes={classes} onClick={() => changeName(PreviusName.id, name, PreviusName)} type="button" isLoading={isLoading} disabled={isDisabled} required={false} >Zmień nazwę</Button>
                <button onClick={() => SetSelectedParent(null)}>Anuluj</button>
            </div>
        </div>)
}