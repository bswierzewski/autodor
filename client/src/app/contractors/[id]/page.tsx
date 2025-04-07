'use client';

import { AnyFieldApi, useForm } from '@tanstack/react-form';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';
import { z } from 'zod';

import { useGetContractor, useUpdateContractor } from '@/lib/api/endpoints/contractors';
import { updateContractorBody } from '@/lib/api/endpoints/contractors.zod';

import FormButtons from '@/components/FormButtons';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

function FieldInfo({ field }: { field: AnyFieldApi }) {
  return (
    <>
      {field.state.meta.isTouched && field.state.meta.errors.length ? (
        <em className="text-red-500">{field.state.meta.errors.map((err) => err.message).join(',')}</em>
      ) : null}
      {field.state.meta.isValidating ? 'Validating...' : null}
    </>
  );
}

export type updateContractorSchema = z.infer<typeof updateContractorBody>;

export default function UpdateContractor({ params }: { params: { id: string } }) {
  const router = useRouter();
  const { data } = useGetContractor(Number(params.id), { query: { gcTime: 0 } });

  const { mutate, isPending } = useUpdateContractor({
    mutation: {
      onSuccess() {
        toast.success('Zaktualizowano kontrahenta.');
        router.back();
      }
    }
  });

  const form = useForm({
    defaultValues: {
      id: Number(params.id),
      name: data?.name ?? '',
      city: data?.city ?? '',
      nip: data?.nip ?? '',
      zipCode: data?.zipCode ?? '',
      street: data?.street ?? '',
      email: data?.email ?? ''
    } as updateContractorSchema,
    onSubmit: (values) => {
      mutate({
        id: Number(params.id),
        data: values.value
      });
    },
    validators: {
      onSubmit: updateContractorBody
    }
  });

  if (!data) return <div>Loading...</div>;

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        e.stopPropagation();
        form.handleSubmit();
      }}
      className="mt-5"
    >
      <div className="grid w-full items-center gap-4">
        <div className="flex flex-col space-y-2">
          <form.Field name="name">
            {(field) => (
              <>
                <Label htmlFor="name">Nazwa</Label>
                <Input
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Nazwa"
                />
                <FieldInfo field={field} />
              </>
            )}
          </form.Field>
        </div>
        <div className="flex flex-col space-y-2">
          <form.Field name="city">
            {(field) => (
              <>
                <Label htmlFor="city">Miasto</Label>
                <Input
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Miasto"
                />
                <FieldInfo field={field} />
              </>
            )}
          </form.Field>
        </div>
        <div className="flex flex-col space-y-2">
          <form.Field name="street">
            {(field) => (
              <>
                <Label htmlFor="street">Ulica</Label>
                <Input
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Ulica"
                />
                <FieldInfo field={field} />
              </>
            )}
          </form.Field>
        </div>
        <div className="flex flex-col space-y-2">
          <form.Field name="nip">
            {(field) => (
              <>
                <Label htmlFor="nip">NIP</Label>
                <Input
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="NIP"
                />
                <FieldInfo field={field} />
              </>
            )}
          </form.Field>
        </div>
        <div className="flex flex-col space-y-2">
          <form.Field name="zipCode">
            {(field) => (
              <>
                <Label htmlFor="zipCode">Kod pocztowy</Label>
                <Input
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="00-000"
                />
                <FieldInfo field={field} />
              </>
            )}
          </form.Field>
        </div>
        <div className="flex flex-col space-y-2">
          <form.Field name="email">
            {(field) => (
              <>
                <Label htmlFor="email">Email</Label>
                <Input
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Email"
                />
                <FieldInfo field={field} />
              </>
            )}
          </form.Field>
        </div>
      </div>
      <FormButtons isPending={isPending} mode="update" />
    </form>
  );
}
