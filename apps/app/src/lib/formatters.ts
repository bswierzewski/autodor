import dayjs from "dayjs";

export const currencyFormatter = new Intl.NumberFormat("pl-PL", {
	style: "currency",
	currency: "PLN",
	minimumFractionDigits: 2,
	maximumFractionDigits: 2,
});

export function formatDate(date: Date | string): string {
	return dayjs(date).format("YYYY-MM-DD");
}

export function formatCurrency(value: number | string): string {
	return currencyFormatter.format(Number(value));
}
