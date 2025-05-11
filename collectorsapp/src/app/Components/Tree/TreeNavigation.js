"use client";

import "./SideMenuTreeComponent.css";
import { useEffect, useState, useRef } from "react";
import Tree from "./Tree";
import { buildTree } from "./BuildTree";
import { useSession } from "next-auth/react";
import { useMenuItemsProvider } from "@/app/Providers/MenuProvider/MenuProvider";
import { toast } from "react-toastify";
import { useRouter, usePathname, redirect } from "next/navigation";
import Image from "next/image";
import EditData from "../TreeItemInteractions/EditItem.js";
import AddTreeItem from "../TreeItemInteractions/AddItem";
import DeleteItemConfirm from "../TreeItemInteractions/DeleteItem"

export default function TreeView({ onMenuInfo }) {
  const router = useRouter();

  const { data: session, status } = useSession();
  const { menuItems, error, addMenuItem, deleteMenuItem, updateMenuItem } = useMenuItemsProvider();
  
  const params = usePathname();
  const [expandedNodes, setExpandedNodes] = useState(new Set());
  const [newNodeName, setNewNodeName] = useState("");
  const [selectedParent, setSelectedParent] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isAdding, setIsAdding] = useState(false);
  const [isChoiceActive, setIsChoiceActive] = useState(false);
  const [name, setName] = useState(null);
  
  const useOnClickOutside = (callback) => {
    const ref = useRef(null);
    useEffect(() => {
      const handleClickOutside = (event) => {
        if (ref.current && !ref.current.contains(event.target)) {
          callback(event);
        }
      };

      document.addEventListener('mousedown', handleClickOutside);
      document.addEventListener('touchstart', handleClickOutside);

      return () => {
        document.removeEventListener('mousedown', handleClickOutside);
        document.removeEventListener('touchstart', handleClickOutside);
      };
    }, [callback]);

    return ref;
  }

  useEffect(() => {
    if (params === "/ViewItems" && Array.isArray(menuItems) && menuItems) {
        const sortedItems = menuItems.filter((item) => item.parentId == 0)
            .sort((a, b) => a.name.localeCompare(b.name))[0]
        if (sortedItems) {
            router.push(`/ViewItems/${sortedItems.name}/${sortedItems.id}`)
        }
        else {
          EditType("add");
          handleSelectParent("null");
        }
    }
}, [menuItems])

  const handleClickOutside = () => {
    if (isChoiceActive) {
      setIsChoiceActive(false);
    }
  };

  const ref = useOnClickOutside(handleClickOutside);

  const toggleNode = (id) =>
    setExpandedNodes((prev) => {
      const newSet = new Set(prev);
      newSet.has(id) ? newSet.delete(id) : newSet.add(id);
      return newSet;
    });

  const handleSelectParent = (node) => setSelectedParent(node);

  useEffect(() => {
    if (error) {
      if (error == "Request failed with status code 409") {
        toast("Taki element już istnieje", { autoClose: false });
      } else {
        toast(error, { autoClose: false })
      }
    }
  }, [error]);

  const addNode = async (parentId, name) => {
    if (!name.trim()) return;
    let parentDepth = 0;
    let parentName = "";
    if (parentId !== "null") {
      const parentNode = menuItems.find((item) => item.id === parentId);
      parentDepth = parentNode?.depth ?? 0;
      parentName = parentNode?.name;
    }
    const itemData = {
      name: name,
      parentId: parentId === "null" ? 0 : parentId,
      parentName: parentName,
      depth: parentId === "null" ? 0 : parentDepth + 1,
      OwnerId: session.user.id,
    };
    const addItemToast = toast("Dodawanie Elementu", { autoClose: false });
    setIsLoading(true);
    await addMenuItem(itemData);
    toast.update(addItemToast, { autoClose: 3000 });
    setNewNodeName("");
    setSelectedParent(null);
    setIsLoading(false);
  };

  const DeleteItem = async (id) => {
    setIsLoading(true)
    var deleteItemToast = toast("Usuwanie elementu", { autoClose: false });
    await deleteMenuItem(id);
    toast.update(deleteItemToast, { autoClose: 3000 });
    setIsLoading(false);
    setSelectedParent(null)
  }

  const ChangeName = async (id, newName, item) => {
    setIsLoading(true);
    var updateItemName = toast("Aktualizowanie nazwy", { autoClose: false });
    item.name = newName;
    await updateMenuItem(id, item);
    toast.update(updateItemName, { autoClose: 3000 });
    setIsLoading(false);
    setSelectedParent(null)
  }

  const EditType = (value) => {
    if (value == "edit") {
      setIsAdding(false);
      setIsEditing(true);
      setIsDeleting(false);
    }
    if (value == "add") {
      setIsAdding(true);
      setIsEditing(false);
      setIsDeleting(false);
    }
    if (value == "delete") {
      setIsAdding(false);
      setIsEditing(false);
      setIsDeleting(true);
    }
    setSelectedParent(null);
    setIsChoiceActive(false);
  }

  if (status === "loading") return null;
  
  if (status === "authenticated" && menuItems) {
  
    const treeData = buildTree(menuItems)
  
    return (
      <div className="menu-tree-container">
        <div>
          <div className="menu-collections">
            <h2>Kolekcje</h2>
            <button className="b-visibility" onClick={onMenuInfo}>
              <Image className="close-svg  icon" src="/close.svg" width={44} height={44} alt="close" />
            </button>
          </div>
          <div>
            <Tree
              nodes={treeData}
              expandedNodes={expandedNodes}
              toggleNode={toggleNode}
              handleSelectParent={handleSelectParent}
              selectedParent={selectedParent}
              isEditing={isEditing}
              isDeleting={isDeleting}
              isAdding={isAdding}
            />
            {isAdding && (
              <button
                className="button-edit-node icon"
                onClick={() => handleSelectParent("null")}
                style={{ marginLeft: "10px" }}>
                <Image src={"/add.svg"} height={20} width={20} alt={`add node for root element`} />
              </button>
            )}
            {selectedParent && (<>
              {isDeleting && (
                <DeleteItemConfirm isLoading={isLoading} SetSelectedItem={setSelectedParent} SelectedParent={selectedParent} DeleteItem={DeleteItem} />
              )}
              {isEditing && (<>
                <EditData SetSelectedParent={setSelectedParent} changeName={ChangeName} PreviusName={selectedParent} setName={(e) => setName(e)} classes={""} isLoading={isLoading} isDisabled={isLoading} name={name} />
              </>)}
              {isAdding && (
                <AddTreeItem isLoading={isLoading} setSelectedParent={setSelectedParent} addNode={(parentId, name) => addNode(parentId, name)} newNodeName={newNodeName} setNewNodeName={setNewNodeName} selectedParent={selectedParent} menuItems={menuItems} />
              )}
            </>
            )}
          </div>

        </div>
        {(Array.isArray(menuItems)&& menuItems.length>0) &&<div className="edit-button">
          {(isEditing || isDeleting || isAdding) ? (<div tabIndex={1} role="button"
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                setIsAdding(false); setIsDeleting(false); setIsEditing(false); setIsChoiceActive(false); setSelectedParent(null)
              }
            }}
          >
            <Image src={"/accept.svg"} className="edit-svg icon" onClick={() => { setIsAdding(false); setIsDeleting(false); setIsEditing(false); setIsChoiceActive(false); setSelectedParent(null) }} width={32} height={32} alt="edit" tabIndex={1} />
          </div>
          ) : (<div tabIndex={1} role="button" onKeyDown={(e) => {
            if (e.key === "Enter") {
              setIsChoiceActive(!isChoiceActive)
            }
          }}>
            <Image className="edit-svg icon" src={"/edit.svg"} onClick={() => setIsChoiceActive(!isChoiceActive)} width={32} height={32} alt="accept" tabIndex={1} />
          </div>
          )}
          <div ref={ref} className={isChoiceActive ? ("edit-menu-choice-open") : ("edit-menu-choice-closed")}>
            <div tabIndex={1}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  EditType("add")
                }
              }}
              onClick={() => EditType("add")}>
              Dodaj element
            </div>
            <div tabIndex={1}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  EditType("edit");
                }
              }}
              onClick={() => EditType("edit")}>
              Edytuj element
            </div>
            <div
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  EditType("delete")
                }
              }}
              onClick={() => EditType("delete")}>
              Usuń element
            </div>
          </div>
        </div>}
      </div>
    );
  }
  if (status === "unauthenticated") {
    return (
      <div style={{ textWrap: "wrap" }}>
        Wystąpił błąd, załaduj stronę ponownie.
      </div>
    );
  }
}