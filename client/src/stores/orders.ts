import { create } from 'zustand';

import { OrderDto } from '@/lib/api/autodor';

type OrderDtoExtended = OrderDto & {
  isSelected: boolean;
  isVisible: boolean;
};

type State = {
  searchTerm: string;
  orders: OrderDtoExtended[];
};

type Actions = {
  setSearchTerm: (searchTerm: string) => void;
  setOrders: (orders: OrderDto[] | undefined) => void;
  toggleOrder: (order: OrderDtoExtended) => void;
  toggleOrders: (isSelected: boolean) => void;
  excludeOrder: (orderId: string) => void;
};

const initialState: State = {
  searchTerm: '',
  orders: []
};

export const useOrdersStore = create<State & Actions>((set, get) => ({
  ...initialState,

  setSearchTerm: (searchTerm) =>
    set((state) => ({
      searchTerm,
      orders: state.orders.map((order) => ({
        ...order,
        isVisible:
          (order.person?.toLowerCase().includes(searchTerm.toLowerCase()) ?? true) ||
          (order.customerNumber?.toLowerCase().includes(searchTerm.toLowerCase()) ?? true)
      }))
    })),

  setOrders: (orders = []) =>
    set({
      searchTerm: '',
      orders: orders.map((order) => ({
        ...order,
        isSelected: false,
        isVisible: true
      }))
    }),

  toggleOrder: (orderExtended) =>
    set((state) => {
      const updatedOrders = state.orders.map((order) =>
        order.id === orderExtended.id ? { ...order, isSelected: !order.isSelected } : order
      );

      return {
        orders: updatedOrders
      };
    }),

  toggleOrders: (isSelected) =>
    set((state) => {
      const orders = state.orders.map((order) => ({ ...order, isSelected: isSelected }));

      return {
        orders
      };
    }),
  excludeOrder: (orderId) =>
    set((state) => {
      const updatedOrders = state.orders.map((order) =>
        order.id === orderId ? { ...order, isExcluded: !order.isExcluded } : order
      );

      return {
        orders: updatedOrders
      };
    })
}));
