'use client';

import { useContractorsStore } from '@/stores/contractor';
import { useOrdersStore } from '@/stores/orders';
import { BookPlus, Loader2 } from 'lucide-react';
import moment from 'moment';
import { useState } from 'react';
import toast from 'react-hot-toast';
import { z } from 'zod';

import { useCreateInvoice } from '@/lib/api/endpoints/invoices';

import ContractorPopover from './components/ContractorPopover';
import OrdersTable from './components/OrdersTable';
import { DatePicker } from '@/components/DatePicker';
import Errors from '@/components/Errors';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';

const invoiceSchema = z.object({
  orders: z
    .array(
      z.object({
        id: z.string()
      })
    )
    .min(1, { message: 'Wymagane jest wybranie przynajmniej jednego zamówienia' }),
  contractor: z.object(
    {
      id: z.string()
    },
    {
      required_error: 'Kontrahent jest wymagany'
    }
  ),
  issueDate: z.string().min(1),
  saleDate: z.string().min(1)
});

type InvoiceData = z.infer<typeof invoiceSchema>;
type InvoiceErrors = z.ZodFormattedError<InvoiceData>;

export default function Dashboard() {
  const [issueDate, setIssueDate] = useState<Date | undefined>(new Date());
  const [saleDate, setSaleDate] = useState<Date | undefined>(new Date());
  const [invoiceNumber, setInvoiceNumber] = useState<number | undefined>();
  const [errors, setErrors] = useState<InvoiceErrors>();

  const orders = useOrdersStore((state) => state.orders);
  const selectedContractor = useContractorsStore((state) => state.selectedContractor);

  const { mutate, isPending } = useCreateInvoice({
    mutation: {
      onSuccess(data, variables, context) {
        if (data.response?.kod === 0) toast.success(data.response?.informacja ?? 'Pusta odpowiedź');
        else toast.error(`${data.response?.kod} - ${data.response?.informacja}`);
      }
    }
  });

  const handleCreateInvoice = () => {
    const invoiceData = {
      orders: orders.filter((order) => order.isSelected && order.isVisible),
      contractor: selectedContractor,
      invoiceNumber: invoiceNumber,
      issueDate: moment(issueDate).format(),
      saleDate: moment(saleDate).format()
    };

    const result = invoiceSchema.safeParse(invoiceData);

    if (!result.success) {
      setErrors(result.error.format());
      return;
    }

    setErrors(undefined);

    mutate({
      data: invoiceData
    });
  };

  return (
    <div className="flex flex-col xl:flex-row gap-5">
      <div className="flex-1">
        <OrdersTable />
        <Errors errors={errors?.orders?._errors} />
      </div>
      <div className="xl:flex flex-row gap-5">
        <Separator orientation="vertical" className="hidden xl:inline" />
        <div className="flex flex-col gap-5 mt-2">
          <div className="flex-1 xl:flex-none">
            <Label>Numer faktury</Label>
            <Input
              placeholder="Numer faktury"
              value={invoiceNumber}
              type="number"
              onChange={(e) => setInvoiceNumber(Number(e.target.value))}
            />
          </div>
          <div>
            <DatePicker date={issueDate} setDate={setIssueDate} label="Data faktury" />
            <Errors errors={errors?.issueDate?._errors} />
          </div>
          <div>
            <DatePicker date={saleDate} setDate={setSaleDate} label="Data sprzedaży" />
            <Errors errors={errors?.saleDate?._errors} />
          </div>
          <div className="flex-1 xl:flex-none flex flex-col gap-2">
            <Label>Kontrahent</Label>
            <ContractorPopover />
            <Errors errors={errors?.contractor?._errors} />
          </div>
          {isPending ? (
            <Button className="xl:mt-6" disabled>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Proszę czekać
            </Button>
          ) : (
            <Button className="xl:mt-6" size="default" onClick={handleCreateInvoice}>
              <BookPlus />
              <span className="ml-3">Wystaw fakturę</span>
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
