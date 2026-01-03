import { create } from 'zustand';

import { OrderItem } from '@/lib/api/models';

type State = {
  items: OrderItem[];
};

type Actions = {
  setItems: (items: OrderItem[]) => void;
  excludeItem: (partNumber: string) => void;
};

const initialState: State = {
  items: []
};

export const useOrderItemsStore = create<State & Actions>((set) => ({
  ...initialState,

  setItems: (items) =>
    set({
      items: items
    }),

  excludeItem: (partNumber) =>
    set((state) => {
      const updatedItems = state.items.map((item) =>
        item.partNumber === partNumber ? { ...item, isExcluded: !item.isExcluded } : item
      );

      return {
        items: updatedItems
      };
    })
}));
