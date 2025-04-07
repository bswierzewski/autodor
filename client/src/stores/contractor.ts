import { create } from 'zustand';

import { Contractor } from '@/lib/api/models';

type State = {
  selectedContractor: Contractor | undefined;
};

type Actions = {
  setSelectedContractor: (contractor: Contractor) => void;
};

const initialState: State = {
  selectedContractor: undefined
};

export const useContractorsStore = create<State & Actions>((set) => ({
  ...initialState,

  setSelectedContractor: (contractor) => set({ selectedContractor: contractor })
}));
