import { createFileRoute } from "@tanstack/react-router";
import { useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
	useGetContractors,
	useCreateContractor,
	getGetContractorsQueryKey,
} from "@/api/contractors/contractors";

export const Route = createFileRoute("/")({
	component: Index,
});

const schema = z.object({
	name: z.string().min(1, "Nazwa jest wymagana"),
	nip: z.string().length(10, "NIP musi mieć 10 cyfr"),
	street: z.string().min(1, "Ulica jest wymagana"),
	city: z.string().min(1, "Miasto jest wymagane"),
	zipCode: z.string().regex(/^\d{2}-\d{3}$/, "Format: XX-XXX"),
	email: z.string().email("Nieprawidłowy email"),
});

type FormValues = z.infer<typeof schema>;

function Index() {
	const queryClient = useQueryClient();

	const {
		register,
		handleSubmit,
		reset,
		formState: { errors },
	} = useForm<FormValues>({ resolver: zodResolver(schema) });

	const { data: contractors, isPending, isError } = useGetContractors();

	const { mutate: createContractor, isPending: isCreating } =
		useCreateContractor({
			mutation: {
				onSuccess: () => {
					queryClient.invalidateQueries({
						queryKey: getGetContractorsQueryKey(),
					});
					reset();
				},
			},
		});

	function onSubmit(data: FormValues) {
		createContractor({ data });
	}

	return (
		<div className="container mx-auto p-8 space-y-8">
			<h1 className="text-4xl font-bold">Kontrahenci</h1>

			<Card className="max-w-2xl">
				<CardHeader>
					<CardTitle>Dodaj kontrahenta</CardTitle>
				</CardHeader>
				<CardContent>
					<form
						onSubmit={handleSubmit(onSubmit)}
						className="grid grid-cols-2 gap-4"
					>
						<div className="space-y-1">
							<Input placeholder="Nazwa" {...register("name")} />
							{errors.name && (
								<p className="text-xs text-destructive">
									{errors.name.message}
								</p>
							)}
						</div>

						<div className="space-y-1">
							<Input placeholder="NIP" {...register("nip")} />
							{errors.nip && (
								<p className="text-xs text-destructive">{errors.nip.message}</p>
							)}
						</div>

						<div className="space-y-1">
							<Input placeholder="Ulica" {...register("street")} />
							{errors.street && (
								<p className="text-xs text-destructive">
									{errors.street.message}
								</p>
							)}
						</div>

						<div className="space-y-1">
							<Input placeholder="Miasto" {...register("city")} />
							{errors.city && (
								<p className="text-xs text-destructive">
									{errors.city.message}
								</p>
							)}
						</div>

						<div className="space-y-1">
							<Input
								placeholder="Kod pocztowy (XX-XXX)"
								{...register("zipCode")}
							/>
							{errors.zipCode && (
								<p className="text-xs text-destructive">
									{errors.zipCode.message}
								</p>
							)}
						</div>

						<div className="space-y-1">
							<Input placeholder="Email" type="email" {...register("email")} />
							{errors.email && (
								<p className="text-xs text-destructive">
									{errors.email.message}
								</p>
							)}
						</div>

						<Button type="submit" disabled={isCreating} className="col-span-2">
							{isCreating ? "Zapisywanie..." : "Dodaj"}
						</Button>
					</form>
				</CardContent>
			</Card>

			{isPending && <p className="text-muted-foreground">Ładowanie...</p>}
			{isError && <p className="text-destructive">Błąd pobierania danych.</p>}

			<div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
				{contractors?.map((c) => (
					<Card key={c.id}>
						<CardHeader>
							<CardTitle>{c.name}</CardTitle>
						</CardHeader>
						<CardContent className="text-sm text-muted-foreground space-y-1">
							<p>NIP: {c.nip}</p>
							<p>
								{c.street}, {c.zipCode} {c.city}
							</p>
							<p>{c.email}</p>
						</CardContent>
					</Card>
				))}
			</div>
		</div>
	);
}
