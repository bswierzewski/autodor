import { cn } from '@/lib/utils';
import { useContractorsStore } from '@/stores/contractor';
import { Check, ChevronsUpDown } from 'lucide-react';
import { useState } from 'react';

import { useGetContractors } from '@/lib/api/endpoints/contractors';

import { Button } from '@/components/ui/button';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';

export default function ContractorPopover() {
  const [open, setOpen] = useState(false);

  const selectedContractor = useContractorsStore((state) => state.selectedContractor);
  const setSelectedContractor = useContractorsStore((state) => state.setSelectedContractor);

  const { data: contractors } = useGetContractors();

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button variant="outline" role="combobox" aria-expanded={open} className="flex-1 justify-between">
          {selectedContractor?.id ? selectedContractor.name : 'Wybierz kontrahenta...'}
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="p-0">
        <Command
          filter={(value, search) => {
            if (value.toLowerCase().includes(search.toLowerCase())) return 1;
            return 0;
          }}
        >
          <CommandInput placeholder="Wyszukaj kontrahenta..." />
          <CommandEmpty>Nie znaleziono kontrahenta.</CommandEmpty>
          <CommandGroup>
            <CommandList>
              {contractors?.map((contractor) => (
                <CommandItem
                  key={contractor.id}
                  value={contractor.name ?? ''}
                  onSelect={() => {
                    setOpen(false);
                    setSelectedContractor(contractor);
                  }}
                >
                  <Check className={cn('mr-2 h-4 w-4', selectedContractor?.id === contractor.id ? 'opacity-100' : 'opacity-0')} />
                  {contractor.name}
                </CommandItem>
              ))}
            </CommandList>
          </CommandGroup>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
