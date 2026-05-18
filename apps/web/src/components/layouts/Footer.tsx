const buildInfo = import.meta.env as ImportMetaEnv & {
	readonly VITE_APP_GIT_SHA?: string;
	readonly VITE_APP_BUILD_TIME?: string;
};

const buildSha = buildInfo.VITE_APP_GIT_SHA?.trim() || "local";
const buildTime = buildInfo.VITE_APP_BUILD_TIME?.trim() || undefined;
const buildTimeLabel = buildTime?.replace("T", " ").replace("Z", " UTC") || undefined;
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
						<span className="font-medium text-foreground">Build</span>{" "}
						<span className="font-mono">{buildSha.slice(0, 7)}</span>
					</p>
					{buildTimeLabel ? (
						<p>
							<span className="font-medium text-foreground">Built</span>{" "}
							<time className="font-mono" dateTime={buildTime}>
								{buildTimeLabel}
							</time>
						</p>
					) : null}
				</div>
			</div>
		</footer>
	);
}
