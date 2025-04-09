import { AnyFieldApi } from '@tanstack/react-form';
import { IntlErrorCode, useTranslations } from 'next-intl';

export default function FieldInfo({ field }: { field: AnyFieldApi }) {
  const t = useTranslations('Zod');

  const translateError = (code: string) => {
    try {
      return t(code);
    } catch (error: any) {
      if (error.code === IntlErrorCode.MISSING_MESSAGE) {
        return 'Nieznany błąd walidacji';
      }
      throw error;
    }
  };

  return (
    <>
      {field.state.meta.isTouched && field.state.meta.errors.length ? (
        <em className="text-red-500">{field.state.meta.errors.map((err) => translateError(err.code)).join(', ')}</em>
      ) : null}
      {field.state.meta.isValidating ? 'Validating...' : null}
    </>
  );
}
