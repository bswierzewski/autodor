'use client';

import { useForm } from '@tanstack/react-form';
import { useTranslations } from 'next-intl';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';
import { z } from 'zod';

import { useGetContractor, useUpdateContractor } from '@/lib/api/endpoints/contractors';
import { updateContractorBody } from '@/lib/api/endpoints/contractors.zod';

import FieldInfo from '@/components/FieldInfo';
import FormButtons from '@/components/FormButtons';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

export type updateContractorSchema = z.infer<typeof updateContractorBody>;

export default function UpdateContractor({ params }: { params: { id: string } }) {
  const t = useTranslations('Contractor');
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

  const placeholders = t.raw('form.placeholder');
  const fieldNames = ['name', 'city', 'street', 'nip', 'zipCode', 'email'] as const;

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
        {fieldNames.map((fieldName) => (
          <div key={fieldName} className="flex flex-col space-y-2">
            <form.Field name={fieldName}>
              {(field) => (
                <>
                  <Label htmlFor={field.name}>{t(`form.${fieldName}`)}</Label>
                  <Input
                    id={field.name}
                    name={field.name}
                    value={field.state.value ?? ''}
                    onChange={(e) => field.handleChange(e.target.value)}
                    placeholder={placeholders[fieldName]}
                  />
                  <FieldInfo field={field} />
                </>
              )}
            </form.Field>
          </div>
        ))}
      </div>
      <FormButtons isPending={isPending} mode="update" />
    </form>
  );
}
