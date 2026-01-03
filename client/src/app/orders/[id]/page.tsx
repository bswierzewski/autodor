'use client';

import { useGetOrderById } from '@/lib/api/endpoints/orders';

import OrderItemsTable from './components/OrderItemsTable';

export default function OrderDetail({ params }: { params: { id: string } }) {
  const { data, isLoading } = useGetOrderById(
    params.id,
    {
      query: {
        gcTime: 0
      }
    }
  );

  if (isLoading) return <div className="p-6">Ładowanie...</div>;
  if (!data) return <div className="p-6">Nie znaleziono zamówienia</div>;

  return (
    <div className="container mx-auto py-6">
      <div className="mb-6 flex justify-between flex-wrap gap-6">
        <div className="flex-1 min-w-[200px]">
          <p className="text-sm text-gray-500">Numer zamówienia</p>
          <p className="text-lg font-semibold">{data.number}</p>
        </div>
        <div className="flex-1 min-w-[200px]">
          <p className="text-sm text-gray-500">Data</p>
          <p className="text-lg font-semibold">{data.date ? new Date(data.date).toLocaleDateString('pl-PL') : ''}</p>
        </div>
        <div className="flex-1 min-w-[200px]">
          <p className="text-sm text-gray-500">Kontrahent</p>
          <p className="text-lg font-semibold">{data.person}</p>
        </div>
      </div>

      <OrderItemsTable orderId={params.id} items={data.items ?? []} />
    </div>
  );
}
