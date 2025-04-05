'use client';

import { AnyFieldApi, useForm } from '@tanstack/react-form';
import { useRouter } from 'next/navigation';
import { z } from 'zod';

import { useCreateContractor } from '@/lib/api/endpoints/contractors';
import { createContractorBody } from '@/lib/api/endpoints/contractors.zod';

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

export type createContractorSchema = z.infer<typeof createContractorBody>;

export default function AddContractor() {
  const router = useRouter();
  const { mutate, isPending } = useCreateContractor({
    mutation: {
      onSuccess() {
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
    } as createContractorSchema,
    onSubmit: (values) => {
      mutate({
        data: values.value
      });
    },
    validators: { onSubmit: createContractorBody }
  });

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
                  value={field.state.value}
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
                  value={field.state.value}
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
                  value={field.state.value}
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
                  value={field.state.value}
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
                  value={field.state.value}
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
      <FormButtons isPending={isPending} mode="create" />
    </form>
  );
}
