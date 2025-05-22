"use client";
import Link from "next/link";
import React from "react";
import Image from "next/image";
import Style from "./SideMenuTreeComponent.module.css";
import cn from "classnames";

function Tree({
  nodes,
  expandedNodes,
  toggleNode,
  handleSelectParent,
  selectedParent,
  isEditing,
  isAdding,
  isDeleting,
}) {
  if (!nodes?.length) return null;
  return (
    <ul className={Style.UlContainer} style={{ paddingTop: 5 }}>
      {nodes.map((node) => (
        <li
          key={node.id}
          style={{ paddingTop: 5, marginLeft: `${node.depth * 10}px` }}
          
          
        >
          {node.children?.length ? (
            <span tabIndex={1}
            onKeyDown={(e)=>{if(e.key==="Enter"){
              toggleNode(node.id)
            }}}
              onClick={() => toggleNode(node.id)}
              style={{ cursor: "pointer", marginRight: 5 }}
            >
              {expandedNodes.has(node.id) ? (
                <Image src="/arrow-up.svg" className= {cn(Style.svgColor, 'icon')}  height={16} width={16} alt="" />
              ) : (
                <Image src="/arrow-down.svg" className={cn(Style.svgColor, 'icon')} height={16} width={16} alt="" />
              )}
            </span>
          ) : (
            <Image src="/arrow-down.svg" className={cn(Style.svgColorLast, 'disabledIcon')} height={16} width={16} alt="" />
          )}
          <span className={cn(Style.clickRedirect)}> <Link tabIndex={1} href={`/ViewItems/${node.name}/${node.id}`}>{node.name}</Link></span>
          {isEditing && (
            <button
              className={cn(Style.buttonEditNode, 'icon')}
              onClick={() => handleSelectParent(node)}
              style={{ marginLeft: "10px" }}
            >
              <Image src={"/edit_pen.svg"} height={16} width={16} alt={`edit ${node.name}`} />
            </button>
          )}
          {isAdding && (
            <button
            className={cn(Style.buttonEditNode, 'icon')}
            onClick={() => handleSelectParent(node)}
            style={{ marginLeft: "10px" }}>
              <Image src={"/add.svg"} height={16} width={16} alt={`add node for parent ${node.name}`} />
            </button>
          )}
          {isDeleting && node.children?.length === 0  && (
            <button className={cn(Style.buttonEditNode, 'icon')}
            onClick={() => handleSelectParent(node)}
            style={{ marginLeft: "10px" }}>
              <Image
                src="/remove.svg"
                height={16}
                width={16}
                alt={`remove node ${node.name}`}
              />
            </button>
          )}
          {node.children?.length > 0 && expandedNodes.has(node.id) && (
            <Tree
              nodes={node.children}
              expandedNodes={expandedNodes}
              toggleNode={toggleNode}
              handleSelectParent={handleSelectParent}
              selectedParent={selectedParent}
              isEditing={isEditing}
              isAdding = {isAdding}
              isDeleting = {isDeleting}  
            />
          )}
        </li>
      ))}
    </ul>
  );
}

export default Tree;