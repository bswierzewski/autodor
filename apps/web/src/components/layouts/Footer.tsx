import { QuestionMark } from "@phosphor-icons/react";
import { useGetApplicationVersion } from "#/api/system/system";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
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
	const { data: applicationVersion } = useGetApplicationVersion({
		query: {
			staleTime: Number.POSITIVE_INFINITY,
			retry: false,
		},
	});

	const appSha = formatSha(ENV.VITE_GIT_SHA);
	const apiSha = formatSha(applicationVersion?.gitSha);

	return (
		<footer className="border-t bg-background/95 px-4 py-4 text-sm text-muted-foreground lg:px-6">
			<div className="mx-auto flex w-full max-w-screen-2xl flex-wrap items-center justify-between gap-4">
				<p className="min-w-max whitespace-nowrap text-xs text-muted-foreground/90">
					<span>
						Copyright © {currentYear} <span className="font-medium text-foreground">bswierzewski</span>
					</span>
				</p>
				<Popover>
					<PopoverTrigger asChild>
						<button
							type="button"
							className="inline-flex size-7 items-center justify-center rounded-full border border-border/70 bg-background text-muted-foreground transition-colors hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
							aria-label="Show application versions"
						>
							<QuestionMark size={14} weight="bold" />
						</button>
					</PopoverTrigger>
					<PopoverContent align="end" className="w-40 p-3">
						<div className="space-y-1 text-xs text-muted-foreground/90">
							<p>
								APP: <span className="font-mono text-foreground">{appSha}</span>
							</p>
							<p>
								API: <span className="font-mono text-foreground">{apiSha}</span>
							</p>
						</div>
					</PopoverContent>
				</Popover>
			</div>
		</footer>
	);
}
