"use client"
import Tree from "@/app/Components/Tree/TreeNavigation.js"
import "./ContentLayout.css"
import { useState } from "react"
import { useMenu } from "@/app/Providers/MobileMenuProvider"
export default function Content({ children }) {

  const [data, setData] = useState();

  const { isOpen, closeMenu } = useMenu();

  const handleClickAction = (parentId) => setData(parentId);

  return (
    <div className="content-container">
      <div className={`Side-Menu ${isOpen ? 'open' : 'closed'}`}>
        <Tree onButtonClick={handleClickAction} onMenuInfo={closeMenu} />
      </div>
      <div className="Content-Layout-Style">{data}
        {children}
      </div>
    </div>
  );
}