import { useGetVersion } from "#/api/system/system";

const buildInfo = import.meta.env as ImportMetaEnv & {
	readonly VITE_APP_GIT_SHA?: string;
};

const webSha = buildInfo.VITE_APP_GIT_SHA?.trim() || null;
const currentYear = new Date().getFullYear();

export function Footer() {
	const { data: api } = useGetVersion({ query: { staleTime: Number.POSITIVE_INFINITY } });

	return (
		<footer className="border-t bg-background/95 px-4 py-4 text-sm text-muted-foreground lg:px-6">
			<div className="mx-auto flex w-full max-w-screen-2xl flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
				<div className="space-y-1">
					<p className="font-medium text-foreground">Autodor</p>
					<p className="text-xs text-muted-foreground/90">© {currentYear} Autodor. Internal operations platform.</p>
				</div>
				<div className="flex flex-col gap-1 text-xs sm:items-end">
					<p>
						<span className="font-medium text-foreground">Frontend</span>{" "}
						<span className="font-mono">{webSha?.slice(0, 7) ?? "local"}</span>
					</p>
					<p>
						<span className="font-medium text-foreground">API</span>{" "}
						<span className="font-mono">{api?.sha?.slice(0, 7) ?? "—"}</span>
					</p>
				</div>
			</div>
		</footer>
	);
}
