'use server'

import { Auction, PagedResult } from "@/types";
import { getTokenWorkAround } from "./authActions";
import { fetchWrapper } from "@/lib/fetchWrapper";
import { FieldValue, FieldValues } from "react-hook-form";
import { revalidatePath } from "next/cache";

export async function getData(query: string) : Promise<PagedResult<Auction>>{
    return await fetchWrapper.get(`search${query}`)
}

export async function updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1
    }

    const token = await getTokenWorkAround();
    
    return await fetchWrapper.put('auctions/642fa911-b04b-43bb-be7e-31707b970a4b',
    data);

}

export async function createAuction(data: FieldValues) {
    return await fetchWrapper.post('auctions',data);
}

export async function getDetailedViewData(id: string) : Promise<Auction> {
    return await fetchWrapper.get(`auctions/${id}`);
}

export async function updateAuction(data: FieldValues,id: string) {
    const res = await fetchWrapper.put(`auctions/${id}`,data);
    revalidatePath(`/auctions/${id}`);
    return res;

}