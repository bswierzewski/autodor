import { handleDownload } from '@/lib/utils';
import { useOrdersStore } from '@/stores/orders';
import { format } from 'date-fns';
import { BookmarkCheck, BookmarkX, Printer, RotateCw } from 'lucide-react';
import { useEffect, useState } from 'react';
import toast from 'react-hot-toast';

import { usePrintInvoice } from '@/lib/api/endpoints/invoices';
import { useExcludeOrder, useGetOrders } from '@/lib/api/endpoints/orders';

import { DatePicker } from '@/components/DatePicker';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';

export default function OrdersTable() {
  var daysInSecond = 7 * 24 * 60 * 60 * 1000;

  const [dateFrom, setDateFrom] = useState<Date | undefined>(new Date(Date.now() - daysInSecond));
  const [dateTo, setDateTo] = useState<Date | undefined>(new Date());
  const [checked, setChecked] = useState<boolean>(false);
  const [clickedPrinterIds, setClickedPrinterIds] = useState<string[]>([]);

  const toggleOrder = useOrdersStore((state) => state.toggleOrder);
  const toggleOrders = useOrdersStore((state) => state.toggleOrders);
  const searchTerm = useOrdersStore((state) => state.searchTerm);
  const setSearchTerm = useOrdersStore((state) => state.setSearchTerm);
  const orders = useOrdersStore((state) => state.orders);
  const setOrders = useOrdersStore((state) => state.setOrders);
  const excludeOrder = useOrdersStore((state) => state.excludeOrder);

  const { data, refetch, isFetching } = useGetOrders(
    {
      dateFrom: dateFrom?.toDateString() ?? '',
      dateTo: dateTo?.toDateString() ?? ''
    },
    {
      query: {
        enabled: false,
        gcTime: 0
      }
    }
  );

  const { mutate } = useExcludeOrder({
    mutation: {
      onSuccess(data, variables, context) {
        excludeOrder(variables.data.orderId);
        toast.success('Wykluczenie przebiegło pomyślnie');
      },
      onError(error) {
        toast.error('Wystąpił problem podczas wykluczania zamówienia');
      }
    }
  });

  const { mutate: mutatePrint } = usePrintInvoice();

  useEffect(() => {
    setOrders(data);
  }, [data]);

  useEffect(() => {
    toggleOrders(checked);
  }, [checked]);

  // Sort orders by date in descending order before rendering
  const sortedOrders = [...orders].sort((a, b) => {
    const dateA = a.date ? new Date(a.date).getTime() : 0;
    const dateB = b.date ? new Date(b.date).getTime() : 0;
    return dateB - dateA; // Descending order
  });

  return (
    <>
      <div className="flex flex-col md:flex-row gap-5">
        <DatePicker date={dateFrom} onSelect={setDateFrom} label="Data od" />
        <DatePicker date={dateTo} onSelect={setDateTo} label="Data do" />
        <Button className="md:mt-6" disabled={isFetching} size="default" onClick={() => refetch()}>
          <RotateCw className={isFetching ? 'animate-spin' : ''} />
          <span className="ml-3 inline md:hidden">Odśwież</span>
        </Button>
      </div>
      <Input
        className="my-2"
        placeholder="Wyszukaj po nazwie kontrahenta"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>
              <Checkbox
                onCheckedChange={() => {
                  setChecked(!checked);
                }}
                checked={checked}
              />
            </TableHead>
            <TableHead className="text-right">NIP</TableHead>
            <TableHead className="text-right">Data</TableHead>
            <TableHead className="text-right">Ilość pozycji</TableHead>
            <TableHead className="text-right">Kontrahent</TableHead>
            <TableHead className="text-right">Numer zamówienia</TableHead>
            <TableHead className="text-right">Kwota całkowita</TableHead>
            <TableHead className="text-right"></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {sortedOrders
            .filter((order) => order.isVisible)
            .map((order) => (
              <TableRow key={order.id}>
                <TableCell>
                  <Checkbox
                    id={order.id ?? ''}
                    onCheckedChange={() => toggleOrder(order)}
                    checked={order.isSelected}
                    className="mb-[6px]"
                  />
                </TableCell>
                <TableCell className="text-right">{order.customerNumber}</TableCell>
                <TableCell className="text-right">{format(order.date ?? '', 'yyyy-MM-dd')}</TableCell>
                <TableCell className="text-right">{order.itemsCount}</TableCell>
                <TableCell className="text-right">{order.person}</TableCell>
                <TableCell className="text-right">{order.number}</TableCell>
                <TableCell className="text-right">{order.totalPrice?.toFixed(2)} zł</TableCell>
                <TableCell className="text-right">
                  <div className="flex flex-row gap-5">
                    {order.isExcluded ? (
                      <BookmarkX
                        onClick={() => mutate({ data: { orderId: order.id ?? '' } })}
                        className="cursor-pointer"
                      />
                    ) : (
                      <BookmarkCheck
                        onClick={() => mutate({ data: { orderId: order.id ?? '' } })}
                        className={`cursor-pointer opacity-10`}
                      />
                    )}

                    <Printer
                      onClick={() => {
                        setClickedPrinterIds((prev) => [...prev, order.id ?? '']);
                        mutatePrint(
                          {
                            data: {
                              orderId: order.id ?? '',
                              date: order.date
                            }
                          },
                          {
                            onSuccess(data, variables, context) {
                              handleDownload(data, order.number);
                            },
                            onSettled() {
                              setClickedPrinterIds((prev) => prev.filter((id) => id !== order.id));
                            }
                          }
                        );
                      }}
                      className={`cursor-pointer ${clickedPrinterIds.includes(order.id ?? '') ? 'animate-pulse' : ''}`}
                    />
                  </div>
                </TableCell>
              </TableRow>
            ))}
        </TableBody>
      </Table>
    </>
  );
}
