import Button from "../Button/Button";
import Style from "../Tree/SideMenuTreeComponent.module.css";
import cn from "classnames";

export default function AddTreeItem({ setSelectedParent, selectedParent, menuItems, setNewNodeName, newNodeName, addNode, isLoading = false }) {

return (<>
        <div style={{ marginTop: "10px" }}>
            <label className={cn(Style.flexLabel)}>
                Wybrany rodzic:{" "}
                <select
                    className={cn(Style.selectStyle)}
                    value={selectedParent ? selectedParent.id : ""}
                    onChange={(e) => {
                        const id = e.target.value;
                        if (!id) {
                            setSelectedParent(null);
                        } else {
                            const newParent = menuItems.find(
                                (item) => item.id.toString() === id
                            );
                            setSelectedParent(newParent);
                        }
                    }}>
                    <option value="">
                        {selectedParent && selectedParent.name
                            ? selectedParent.name
                            : "Brak rodzica"}
                    </option>
                    {menuItems.map((item) => (
                        <option key={item.id} value={item.id}>
                            {item.name ? item.name : "Brak rodzica"}
                        </option>
                    ))}
                </select>
            </label>
        </div>
        <div style={{ marginTop: "10px" }}>
            <input
                className={cn(Style.inputStyle)}
                type="text"
                value={newNodeName}
                onChange={(e) => setNewNodeName(e.target.value)}
                placeholder="New node name" />
            <div className={cn(Style.addItemButton)}>
                <Button isLoading={isLoading} type="button"
                    disabled={isLoading}
                    onClick={() => {
                        //console.log(selectedParent.id + " stest")
                        addNode(
                            selectedParent == "null" ? selectedParent : selectedParent.id,
                            newNodeName
                        )
                    }}>
                    Dodaj element
                </Button>
                <button onClick={() => setSelectedParent(null)}>
                    Anuluj
                </button>
            </div>
        </div></>)
}