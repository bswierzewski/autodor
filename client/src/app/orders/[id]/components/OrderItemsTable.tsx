import { useOrderItemsStore } from '@/stores/orderItems';
import { BookmarkCheck, BookmarkX } from 'lucide-react';
import { useEffect } from 'react';
import toast from 'react-hot-toast';

import { useExcludeOrderPosition } from '@/lib/api/endpoints/orders';
import { OrderItem } from '@/lib/api/models';

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';

interface OrderItemsTableProps {
  orderId: string;
  items: OrderItem[];
}

export default function OrderItemsTable({ orderId, items }: OrderItemsTableProps) {
  const setItems = useOrderItemsStore((state) => state.setItems);
  const excludeItem = useOrderItemsStore((state) => state.excludeItem);
  const storeItems = useOrderItemsStore((state) => state.items);

  const { mutate } = useExcludeOrderPosition({
    mutation: {
      onSuccess(data, variables) {
        const partNumber = variables.data.partNumber;
        if (partNumber) {
          excludeItem(partNumber);
        }
        toast.success('Wykluczenie pozycji przebiegło pomyślnie');
      },
      onError() {
        toast.error('Wystąpił problem podczas wykluczania pozycji');
      }
    }
  });

  useEffect(() => {
    setItems(items);
  }, [items, setItems]);

  return (
    <div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Numer części</TableHead>
            <TableHead>Nazwa produktu</TableHead>
            <TableHead className="text-right">Ilość</TableHead>
            <TableHead className="text-right">Akcje</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {storeItems.map((item) => (
            <TableRow key={item.partNumber}>
              <TableCell className="font-medium">{item.partNumber}</TableCell>
              <TableCell>{item.partName}</TableCell>
              <TableCell className="text-right">{item.quantity}</TableCell>
              <TableCell className="text-right">
                {item.isExcluded ? (
                  <BookmarkX
                    onClick={() =>
                      mutate({
                        data: {
                          orderId: orderId,
                          partNumber: item.partNumber ?? ''
                        }
                      })
                    }
                    className="cursor-pointer inline-block"
                  />
                ) : (
                  <BookmarkCheck
                    onClick={() =>
                      mutate({
                        data: {
                          orderId: orderId,
                          partNumber: item.partNumber ?? ''
                        }
                      })
                    }
                    className="cursor-pointer opacity-10 inline-block"
                  />
                )}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
