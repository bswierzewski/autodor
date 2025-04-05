export default function Errors({ errors }: { errors?: string[] }) {
  if (!errors?.length) return null;

  return (
    <div className="text-red-500">
      {errors.map((err, id) => (
        <p key={id}>{err}</p>
      ))}
    </div>
  );
}
