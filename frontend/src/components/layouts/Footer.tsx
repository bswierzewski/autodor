const buildInfo = import.meta.env as ImportMetaEnv & {
	readonly VITE_APP_GIT_SHA?: string;
	readonly VITE_APP_BUILD_TIME?: string;
};

const buildSha = buildInfo.VITE_APP_GIT_SHA?.trim();
const shortBuildSha = buildSha ? buildSha.slice(0, 7) : null;
const buildTime = buildInfo.VITE_APP_BUILD_TIME?.trim();
const buildDate = buildTime ? new Date(buildTime) : null;
const formattedBuildTime =
	buildDate && !Number.isNaN(buildDate.getTime())
		? new Intl.DateTimeFormat("pl-PL", {
				dateStyle: "medium",
				timeStyle: "short",
				timeZone: "UTC",
			}).format(buildDate)
		: (buildTime ?? "lokalny build");
const currentYear = new Date().getFullYear();

export function Footer() {
	return (
		<footer className="border-t bg-background/95 px-4 py-4 text-sm text-muted-foreground lg:px-6">
			<div className="mx-auto flex w-full max-w-screen-2xl flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
				<div className="space-y-1">
					<p className="font-medium text-foreground">Autodor</p>
					<p className="text-xs text-muted-foreground/90">© {currentYear} Autodor. Internal operations platform.</p>
				</div>
				<div className="flex flex-col gap-1 text-xs sm:items-end">
					<p>
						<span className="font-medium text-foreground">Wersja</span>{" "}
						<span className="font-mono">{shortBuildSha ?? "local"}</span>
					</p>
					<p>
						<span className="font-medium text-foreground">Build</span> <span>{formattedBuildTime}</span>
					</p>
				</div>
			</div>
		</footer>
	);
}
