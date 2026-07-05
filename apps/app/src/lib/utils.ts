import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
	return twMerge(clsx(inputs));
}

export function downloadBlob(blob: Blob, fileName: string) {
	// Create a temporary object URL so the browser can treat the in-memory blob like a file.
	const objectUrl = URL.createObjectURL(blob);

	// Use a detached anchor element because the browser download flow is still driven by links.
	const link = document.createElement("a");

	link.href = objectUrl;
	link.download = fileName;

	// The link must be in the DOM for cross-browser download behavior to be reliable.
	document.body.append(link);
	link.click();
	link.remove();

	// Release the object URL once the download has been triggered to avoid leaking memory.
	URL.revokeObjectURL(objectUrl);
}
