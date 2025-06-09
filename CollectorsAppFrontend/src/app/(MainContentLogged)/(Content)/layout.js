"use client"

import Tree from "@/app/Components/Tree/TreeNavigation.js";
import Style from "./ContentLayout.module.css";
import { useState } from "react";
import { useMenu } from "@/app/Providers/MobileMenuProvider";
import cn from "classnames";

export default function Content({ children }) {

  const [data, setData] = useState();

  const { isOpen, closeMenu } = useMenu();

  const handleClickAction = (parentId) => setData(parentId);

  return (
    <div className={cn(Style.contentContainer)}>
      <div className={cn(Style.SideMenu, {[Style.open]:isOpen})}
      //{`Side-Menu ${isOpen ? 'open' : 'closed'}`}
      >
        <Tree onButtonClick={handleClickAction} onMenuInfo={closeMenu} />
      </div>
      <div className={cn(Style.ContentLayoutStyle)}>{data}
        {children}
      </div>
    </div>
  );
}