import { ENV } from "../../config/env.ts";

const currentYear = new Date().getFullYear();

function formatSha(value: string | null | undefined) {
	if (!value) {
		return "unknown";
	}

	const trimmedValue = value.trim();
	if (!trimmedValue) {
		return "unknown";
	}

	return trimmedValue.slice(0, 7);
}

export function Footer() {
	const sha = formatSha(ENV.VITE_GIT_SHA);

	return (
		<footer className="border-t bg-background/95 px-4 py-4 text-sm text-muted-foreground lg:px-6">
			<div className="mx-auto flex w-full max-w-screen-2xl flex-wrap items-center justify-between gap-4">
				<p className="min-w-max whitespace-nowrap text-xs text-muted-foreground/90">
					<span>
						Copyright © {currentYear} <span className="font-medium text-foreground">bswierzewski</span>
					</span>
				</p>
				<p className="whitespace-nowrap text-xs text-muted-foreground/90">{sha}</p>
			</div>
		</footer>
	);
}
