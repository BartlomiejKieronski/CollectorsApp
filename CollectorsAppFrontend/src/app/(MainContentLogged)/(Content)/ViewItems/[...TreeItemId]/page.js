"use client"

import { GetSignedItemImagesUrls, GetPaginatedItems, CollectionItemCount, DeleteMultipleItems } from "@/app/lib/utility";
import { useState, useEffect, useMemo } from "react";
import { useRouter, useParams } from "next/navigation";
import { useSession } from "next-auth/react";
import { toast } from "react-toastify";
import { useMenuItemsProvider } from "@/app/Providers/MenuProvider/MenuProvider";
import Card from "@/app/Components/ItemViewComponents/Card";
import Pagination from "@/app/Components/Pagination/Pagination";
import Style from "../ItemsView.module.css";
import cn from "classnames";
import Button from "@/app/Components/Button/Button";

export default function ItemView() {

    const { data: session } = useSession();
    const { menuItems } = useMenuItemsProvider();

    const [selected, setSelected] = useState([]);
    const [edit, setEdit] = useState(false);
    const [itemData, setItemData] = useState(null);
    const [pathData, setPathData] = useState(null);
    const [itemCount, setItemCount] = useState(null);
    const [itemsPerPage, setItemsPerPage] = useState(10);
    const [page, setPage] = useState(1);
    const [selectedValue, setSelectedValue] = useState(10);
    const [isLoading, setIsLoading] = useState(false);

    const router = useRouter();
    const params = useParams();

    const treeItemId = useMemo(() => params.TreeItemId, [params.TreeItemId]);
    
    useEffect(() => {
        const storedValue = window.localStorage.getItem("display_item_count");
        const initialCount = storedValue ? Number(storedValue) : 10;
        window.localStorage.setItem("display_item_count", initialCount);
        setItemsPerPage(initialCount);
        setSelectedValue(initialCount);
    }, []);

    useEffect(() => {
        const count = Number(selectedValue);
        if (count !== itemsPerPage) {
            setItemsPerPage(count);
        }
    }, [selectedValue]);

    useEffect(() => {
        if (
            session?.user?.id &&
            treeItemId &&
            treeItemId.length >= 2 &&
            !isNaN(Number(treeItemId[1])) &&
            itemsPerPage &&
            menuItems &&
            Array.isArray(menuItems)
        ) {
            fetchData();
        }

    }, [session?.user?.id, treeItemId, itemsPerPage, page, menuItems]);

    async function fetchData() {
        try {
            const [itemRes, itemCt] = await Promise.all([
                GetPaginatedItems(page, session.user.id, treeItemId[1], itemsPerPage),
                CollectionItemCount(treeItemId[1], session.user.id)
            ]);

            const collection = menuItems.find(item => item.id == treeItemId[1]);

            if (collection && (collection.name !== treeItemId[0] || collection.id != treeItemId[1])) {

                router.push(`/ViewItems/${collection.name}/${collection.id}`);
                return;
            }
            setItemData(itemRes.data);
            setItemCount(itemCt.data);

        } catch (err) {
            toast("Wystąpił błąd podczas pobierania informacji z serwera: " + err, { autoClose: false });
        }
    }

    useEffect(() => {
        let isActive = true;
        if (itemData) {
            GetSignedItemImagesUrls(itemData).then(res => {
                if (isActive) {
                    setPathData(res.responseData);
                }
            });
        }
        return () => {
            isActive = false;
        };
    }, [itemData]);

    const handleCheckboxChange = (e, itemId) => {
        if (e.target.checked) {
            setSelected(prev => [...prev, itemId]);
        } else {
            setSelected(prev => prev.filter(id => id !== itemId));
        }
    };

    useEffect(() => {
        if (itemCount && itemsPerPage) {
            const totalPages = Math.ceil(itemCount / itemsPerPage);
            if (page > totalPages) {
                setPage(1);
            }
        }
    }, [itemCount, itemsPerPage, page]);

    const changePage = (selectedPage) => setPage(selectedPage);


    const handleSelectChange = (e) => {
        const value = e.target.value;
        setSelectedValue(value);
        window.localStorage.setItem("display_item_count", value);
    };

    const DeleteItems = async () => {
        setIsLoading(true);
        await DeleteMultipleItems(selected, session.user?.id);
        router.refresh();
        setIsLoading(false);
    }

    const AddButtonRedirect = async() => {
        setIsLoading(true);
        router.push(`/AddItem/${treeItemId[0]}`);
        setIsLoading(false);
    }

    return (
        <div className={cn(Style.cardContainer)}>
            <>
                <div className={cn(Style.controllers)}>
            
                    <div>
                        <label>
                            Liczba pozycji: &nbsp;
                            <select value={selectedValue} onChange={handleSelectChange}>
                                <option value={10}>10</option>
                                <option value={25}>25</option>
                                <option value={50}>50</option>
                            </select>
                        </label>
                    </div>
                    <div>
                        <button onClick={() => { setEdit(!edit); setSelected([]); }}>
                            {edit ? "Zakończ" : "Edytuj"}
                        </button>
                    </div>
                    {!edit && <div>
                        <Button isLoading={isLoading} disabled={isLoading} onClick={async() =>await AddButtonRedirect()}>Dodaj</Button>
                    </div>}
                    {selected.length > 0 && (
                        <div>
                            <Button
                                isLoading={isLoading}
                                disabled={isLoading}
                                onClick={() => DeleteItems()}>
                                Usuń
                            </Button>
                        </div>
                    )}
                </div>
                <div className={cn(Style.cardItemLayout)}>
                    {pathData ? <>{pathData.map(item => (
                        <div key={item.id} className={cn(Style.cardMargin)}>
                            {edit && (
                                <div className={cn(Style.select)}>
                                    <input
                                        type={"checkbox"}
                                        onChange={(e) => handleCheckboxChange(e, item)}
                                        checked={selected.includes(item)}
                                    />
                                </div>
                            )}
                            <Card cardData={item} />
                        </div>
                    ))}</> : <>{pathData && pathData == 0 && <Button isLoading={isLoading} onClick={() => AddButtonRedirect()}>Dodaj Element</Button>}</>}
                </div>
            </>

            {itemCount > itemsPerPage && (
                <div className={cn(Style.pagination)}>
                    <Pagination
                        totalItems={itemCount}
                        itemsPerPage={itemsPerPage}
                        onPageChange={changePage}
                        currentPage={page} />
                </div>
            )}
        </div>
    );
}