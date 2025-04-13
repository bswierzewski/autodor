'use client';

import { useContractorsStore } from '@/stores/contractor';
import { useOrdersStore } from '@/stores/orders';
import { useForm } from '@tanstack/react-form';
import { BookPlus, Loader2 } from 'lucide-react';
import moment from 'moment';
import { useEffect } from 'react';
import toast from 'react-hot-toast';
import { z } from 'zod';

import { useCreateInvoice } from '@/lib/api/endpoints/invoices';
import { createInvoiceBody } from '@/lib/api/endpoints/invoices.zod';

import ContractorPopover from './components/ContractorPopover';
import OrdersTable from './components/OrdersTable';
import { DatePicker } from '@/components/DatePicker';
import FieldInfo from '@/components/FieldInfo';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';

type InvoiceData = z.infer<typeof createInvoiceBody>;

export default function Dashboard() {
  const orders = useOrdersStore((state) => state.orders);
  const selectedContractor = useContractorsStore((state) => state.selectedContractor);

  const { mutate, isPending } = useCreateInvoice({
    mutation: {
      onSuccess(data) {
        if (data.response?.kod === 0) toast.success(data.response?.informacja ?? 'Pusta odpowiedź');
        else toast.error(`${data.response?.kod} - ${data.response?.informacja}`);
      }
    }
  });

  useEffect(() => {
    const selectedOrders = orders.filter((order) => order.isSelected && order.isVisible);
    form.setFieldValue(
      'orderIds',
      selectedOrders.map((order) => order.id ?? '')
    );

    form.setFieldValue(
      'dates',
      selectedOrders.map((order) => order.date ?? '')
    );

    if (selectedOrders.length > 0) form.validateField('orderIds', 'change');
  }, [orders]);

  useEffect(() => {
    form.setFieldValue('contractorId', selectedContractor?.id ?? 0);
    if (selectedContractor) form.validateField('contractorId', 'change');
  }, [selectedContractor]);

  const form = useForm({
    defaultValues: {
      invoiceNumber: undefined,
      saleDate: new Date().toISOString(),
      issueDate: new Date().toISOString(),
      dates: [],
      contractorId: 0,
      orderIds: []
    } as InvoiceData,
    onSubmit: ({ value }) => {
      mutate({ data: value });
    }
  });

  return (
    <div className="flex flex-col xl:flex-row gap-5">
      <div className="flex-1">
        <form.Field
          name="orderIds"
          validators={{
            onSubmit: ({ value }) => (value.length == 0 ? 'Wybierz przynajmniej jedną pozycję' : '')
          }}
        >
          {(field) => (
            <>
              <OrdersTable />
              <FieldInfo field={field} />
            </>
          )}
        </form.Field>
      </div>
      <div className="xl:flex flex-row gap-5">
        <Separator orientation="vertical" className="hidden xl:inline" />
        <div className="flex flex-col gap-5 mt-2">
          <div className="flex-1 xl:flex-none">
            <form.Field name="invoiceNumber">
              {(field) => (
                <>
                  <Label>Numer faktury</Label>
                  <Input
                    placeholder="Numer faktury"
                    type="number"
                    id={field.name}
                    name={field.name}
                    value={field.state.value ?? ''}
                    onChange={(e) => field.handleChange(Number(e.target.value))}
                  />
                  <FieldInfo field={field} />
                </>
              )}
            </form.Field>
          </div>
          <div>
            <form.Field name="issueDate">
              {(field) => (
                <DatePicker
                  date={moment(field.state.value).toDate()}
                  onSelect={(date) => field.handleChange(date?.toString())}
                  label="Data faktury"
                />
              )}
            </form.Field>
          </div>
          <div>
            <form.Field name="saleDate">
              {(field) => (
                <DatePicker
                  date={moment(field.state.value).toDate()}
                  onSelect={(date) => field.handleChange(date?.toString())}
                  label="Data sprzedaży"
                />
              )}
            </form.Field>
          </div>
          <div className="flex-1 xl:flex-none flex flex-col gap-2">
            <form.Field
              name="contractorId"
              validators={{ onSubmit: () => (selectedContractor ? '' : 'Wybierz kontrahenta') }}
            >
              {(field) => (
                <>
                  <Label>Kontrahent</Label>
                  <ContractorPopover />
                  <FieldInfo field={field} />
                </>
              )}
            </form.Field>
          </div>
          {isPending ? (
            <Button className="xl:mt-6" disabled>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Proszę czekać
            </Button>
          ) : (
            <Button className="xl:mt-6" size="default" onClick={form.handleSubmit}>
              <BookPlus />
              <span className="ml-3">Wystaw fakturę</span>
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
