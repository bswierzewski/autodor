import Papa from "papaparse";

export type ManualInvoiceCsvItem = {
	sourceRow: number;
	itemNumber: string;
	quantity: number;
	netUnitPrice: number;
};

type RawManualInvoiceRow = {
	nr_pozycji?: string;
	ilosc?: string;
	cena_netto?: string;
	__parsed_extra?: string[];
};

type ManualInvoiceCsvImportResult = {
	items: ManualInvoiceCsvItem[];
	errors: string[];
};

const expectedHeaders = ["nr_pozycji", "ilosc", "cena_netto"];
const positiveIntegerPattern = /^[1-9]\d*$/;
const positiveDecimalPattern = /^(?:0|[1-9]\d*)(?:,\d+)?$/;

export async function importManualInvoiceCsv(file: File): Promise<ManualInvoiceCsvImportResult> {
	const result = Papa.parse<RawManualInvoiceRow>(await file.text(), {
		delimiter: ";",
		header: true,
		skipEmptyLines: "greedy",
		transformHeader: (header) => header.replace(/^\uFEFF/, "").trim(),
	});
	const fields = result.meta.fields ?? [];

	if (fields.length !== expectedHeaders.length || fields.some((field, index) => field !== expectedHeaders[index])) {
		return {
			items: [],
			errors: [`Nagłówek CSV musi mieć postać: ${expectedHeaders.join(";")}.`],
		};
	}

	const errors = result.errors
		.filter((error) => error.type !== "FieldMismatch")
		.map((error) => `Wiersz ${(error.row ?? 0) + 2}: ${error.message}`);
	const items: ManualInvoiceCsvItem[] = [];

	result.data.forEach((row, index) => {
		const rowNumber = index + 2;
		const itemNumber = row.nr_pozycji?.trim() ?? "";
		const quantityValue = row.ilosc?.trim() ?? "";
		const netUnitPriceValue = row.cena_netto?.trim() ?? "";
		const quantity = Number(quantityValue);
		const netUnitPrice = Number(netUnitPriceValue.replace(",", "."));
		const hasValidItemNumber = itemNumber.length > 0 && itemNumber.length <= 300;
		const hasValidQuantity = positiveIntegerPattern.test(quantityValue) && Number.isSafeInteger(quantity);
		const hasValidNetUnitPrice =
			positiveDecimalPattern.test(netUnitPriceValue) &&
			Number.isFinite(netUnitPrice) &&
			netUnitPrice > 0 &&
			netUnitPrice < 100_000_000;

		if (row.__parsed_extra?.length) {
			errors.push(`Wiersz ${rowNumber}: oczekiwano dokładnie 3 kolumn.`);
			return;
		}
		if (!hasValidItemNumber) {
			errors.push(`Wiersz ${rowNumber}: nr_pozycji musi zawierać od 1 do 300 znaków.`);
		}
		if (!hasValidQuantity) {
			errors.push(`Wiersz ${rowNumber}: ilosc musi być dodatnią liczbą całkowitą.`);
		}
		if (!hasValidNetUnitPrice) {
			errors.push(
				`Wiersz ${rowNumber}: cena_netto musi być dodatnią liczbą mniejszą niż 100 000 000, z przecinkiem dziesiętnym.`,
			);
		}

		if (hasValidItemNumber && hasValidQuantity && hasValidNetUnitPrice) {
			items.push({ sourceRow: rowNumber, itemNumber, quantity, netUnitPrice });
		}
	});

	if (result.data.length === 0) {
		errors.push("Plik CSV nie zawiera żadnych pozycji.");
	}

	return { items: errors.length === 0 ? items : [], errors };
}
