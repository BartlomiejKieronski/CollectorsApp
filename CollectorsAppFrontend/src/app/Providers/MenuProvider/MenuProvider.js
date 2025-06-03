"use client"

import { createContext, useContext, useState, useEffect } from 'react';
import { useSession } from 'next-auth/react';
import instance from '@/app/axiosInstance';
import { updateCollection, deleteCollection } from "@/app/lib/utility"
const MenuContext = createContext();

export function useMenuItemsProvider() {
    return useContext(MenuContext);
}

export function MenuItemsProvider({ children }) {
    const { data: session, status } = useSession();

    const [menuItems, setMenuItems] = useState();
    const [areItemsLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchMenuItems = async () => {
        if (session.user?.id != null) {
            setIsLoading(true);
            setError(null);
            try {
                const response = await instance.get(`api/Collections/GetCollectionsByUserId/${session.user.id}`)
                setMenuItems(response.data)
                setIsLoading(false)
            } catch (err) {
                setError(err.message);
            } finally {
                setIsLoading(false);
            }
        } else {
            setMenuItems(null);
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchMenuItems();
    }, [session]);

    const checkAuth = () => {
        if (status !== 'authenticated') {
            setError("User unauthenticated")
        }
    };

    const addMenuItem = async (newItem) => {
        checkAuth();
        try {
            await instance.post("api/Collections", newItem).then(res => {
                if (res.status == 201) {
                    fetchMenuItems();
                }
            })
        } catch (err) {
            setError(err.message);
        }
    };
    const updateMenuItem = async (id, updatedItem) => {
        checkAuth();
        try {
            await updateCollection(id, updatedItem).then(() => {
                fetchMenuItems();
            })
        } catch (err) {
            setError(err.message);
            throw err;
        }
    };

    const deleteMenuItem = async (id) => {
        checkAuth();
        try {
            await deleteCollection(id, session.user.id).then(async() => {
                await fetchMenuItems();
            })
        } catch (err) {
            setError(err.message);
            throw err;
        }
    };

    return (
        <MenuContext.Provider
            value={{
                menuItems,
                areItemsLoading,
                error,
                addMenuItem: status === 'authenticated' ? addMenuItem : null,
                updateMenuItem: status === 'authenticated' ? updateMenuItem : null,
                deleteMenuItem: status === 'authenticated' ? deleteMenuItem : null,
            }}
        >
            {children}
        </MenuContext.Provider>
    );
}