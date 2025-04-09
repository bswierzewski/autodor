'use client';

import { useForm } from '@tanstack/react-form';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';
import { z } from 'zod';

import { useCreateContractor } from '@/lib/api/endpoints/contractors';
import { createContractorBody } from '@/lib/api/endpoints/contractors.zod';

import FieldInfo from '@/components/FieldInfo';
import FormButtons from '@/components/FormButtons';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

export type createContractorSchema = z.infer<typeof createContractorBody>;

export default function AddContractor() {
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
