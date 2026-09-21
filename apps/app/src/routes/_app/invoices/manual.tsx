import { FileCsvIcon, UploadSimpleIcon } from "@phosphor-icons/react";
import { createFileRoute } from "@tanstack/react-router";
import { useRef, useState } from "react";
import { toast } from "sonner";
import { Button } from "#/components/ui/button";
import { Drawer, DrawerContent, DrawerDescription, DrawerTitle } from "#/components/ui/drawer";
import { CreateManualInvoiceForm } from "#/features/invoicing/components/CreateManualInvoiceForm";
import { ManualInvoiceItemsTable } from "#/features/invoicing/components/ManualInvoiceItemsTable";
import { ManualInvoiceSummary } from "#/features/invoicing/components/ManualInvoiceSummary";
import { importManualInvoiceCsv, type ManualInvoiceCsvItem } from "#/features/invoicing/lib/manualInvoiceCsvImporter";
import { useMediaQuery } from "#/hooks/use-media-query";

export const Route = createFileRoute("/_app/invoices/manual")({
	component: ManualInvoiceRoute,
});

function ManualInvoiceRoute() {
	const isDesktop = useMediaQuery("(min-width: 1024px)");
	const fileInputRef = useRef<HTMLInputElement>(null);
	const [items, setItems] = useState<ManualInvoiceCsvItem[]>([]);
	const [importErrors, setImportErrors] = useState<string[]>([]);
	const [isInvoiceDrawerOpen, setIsInvoiceDrawerOpen] = useState(false);
	const [isImporting, setIsImporting] = useState(false);

	const handleFileChange = async (file: File | undefined) => {
		if (!file) return;

		setIsImporting(true);
		const result = await importManualInvoiceCsv(file);
		setIsImporting(false);

		if (result.errors.length > 0) {
			setImportErrors(result.errors);
			return;
		}

		setItems(result.items);
		setImportErrors([]);
		toast.success(`Zaimportowano ${result.items.length} pozycji.`);
	};

	return (
		<div className="space-y-4">
			<header className="flex flex-col gap-4 rounded-3xl border bg-card p-5 shadow-sm sm:flex-row sm:items-center sm:justify-between">
				<div className="space-y-1">
					<div className="flex items-center gap-2">
						<FileCsvIcon className="text-muted-foreground" size={24} weight="duotone" />
						<h1 className="text-2xl font-semibold tracking-tight">Faktura ręczna</h1>
					</div>
					<p className="text-sm text-muted-foreground">Zaimportuj CSV z nagłówkiem nr_pozycji;ilosc;cena_netto.</p>
				</div>
				<div>
					<input
						ref={fileInputRef}
						accept=".csv,text/csv"
						className="sr-only"
						type="file"
						onChange={(event) => {
							void handleFileChange(event.target.files?.[0]);
							event.target.value = "";
						}}
					/>
					<Button
						className="h-11 w-full px-5 sm:w-auto"
						disabled={isImporting}
						type="button"
						variant="outline"
						onClick={() => fileInputRef.current?.click()}
					>
						<UploadSimpleIcon size={16} />
						{isImporting ? "Importowanie..." : "Importuj CSV"}
					</Button>
				</div>
			</header>

			{importErrors.length > 0 ? (
				<div className="rounded-2xl border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
					{importErrors.map((message) => (
						<p key={message}>{message}</p>
					))}
				</div>
			) : null}

			<ManualInvoiceItemsTable items={items} />
			<ManualInvoiceSummary
				items={items}
				onCreateInvoice={() => {
					setIsInvoiceDrawerOpen(true);
				}}
			/>

			<Drawer
				direction={isDesktop ? "right" : "bottom"}
				open={isInvoiceDrawerOpen}
				onOpenChange={(open) => {
					if (!open) setIsInvoiceDrawerOpen(false);
				}}
			>
				<DrawerContent className={isDesktop ? "px-6 pb-6 [&>div:first-child]:hidden" : "px-4 pb-4"}>
					<DrawerTitle className="sr-only">Wystaw fakturę ręczną</DrawerTitle>
					<DrawerDescription className="sr-only">
						Formularz do wystawienia faktury na podstawie zaimportowanego pliku CSV.
					</DrawerDescription>
					<div className="mt-2 min-h-0 flex-1 overflow-y-auto">
						<CreateManualInvoiceForm
							items={items}
							onCancel={() => setIsInvoiceDrawerOpen(false)}
							onSuccess={() => {
								setIsInvoiceDrawerOpen(false);
								setItems([]);
							}}
						/>
					</div>
				</DrawerContent>
			</Drawer>
		</div>
	);
}
