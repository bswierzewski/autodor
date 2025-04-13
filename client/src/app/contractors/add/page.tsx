'use client';

import { useForm } from '@tanstack/react-form';
import { useTranslations } from 'next-intl';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';

import { useCreateContractor } from '@/lib/api/endpoints/contractors';
import { CreateContractorCommand } from '@/lib/api/models';

import FieldInfo from '@/components/FieldInfo';
import FormButtons from '@/components/FormButtons';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

export default function AddContractor() {
  const t = useTranslations('Contractor');
  const router = useRouter();
  const { mutate, isPending } = useCreateContractor({
    mutation: {
      onSuccess() {
        toast.success('Dodano kontrahenta');
        router.back();
      }
    }
  });

  const form = useForm({
    defaultValues: {
      city: '',
      email: '',
      name: '',
      nip: '',
      street: '',
      zipCode: ''
    } as CreateContractorCommand,
    onSubmit: (values) => {
      mutate({
        data: values.value
      });
    },
    validators: {
      onSubmit: ({ value }) => {
        return {
          fields: {
            name: value.name.length === 0 ? 'Pole jest wymagane' : undefined,
            city: value.city.length === 0 ? 'Pole jest wymagane' : undefined,
            street: value.street.length === 0 ? 'Pole jest wymagane' : undefined,
            nip: value.nip.length === 0 ? 'Pole jest wymagane' : undefined,
            zipCode: value.zipCode.length === 0 ? 'Pole jest wymagane' : undefined
          }
        };
      }
    }
  });

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
      <FormButtons isPending={isPending} mode="create" />
    </form>
  );
}
