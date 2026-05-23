const buildInfo = import.meta.env as ImportMetaEnv & {
	readonly VITE_APP_GIT_SHA?: string;
	readonly VITE_APP_BUILD_TIME?: string;
};

const buildSha = buildInfo.VITE_APP_GIT_SHA?.trim() || "local";
const buildTime = buildInfo.VITE_APP_BUILD_TIME?.trim() || undefined;
const parsedBuildTime = buildTime ? new Date(buildTime) : undefined;
const buildTimeLabel =
	parsedBuildTime && !Number.isNaN(parsedBuildTime.getTime())
		? new Intl.DateTimeFormat(undefined, {
				dateStyle: "medium",
				timeStyle: "short",
			}).format(parsedBuildTime)
		: undefined;
const currentYear = new Date().getFullYear();

export function Footer() {
	return (
		<footer className="border-t bg-background/95 px-4 py-4 text-sm text-muted-foreground lg:px-6">
			<div className="mx-auto flex w-full max-w-screen-2xl items-center justify-between gap-4 overflow-x-auto">
				<p className="min-w-max whitespace-nowrap text-xs text-muted-foreground/90">
					<span>
						Copyright © <span className="font-medium text-foreground">bswierzewski</span>
					</span>
				</p>
				<p className="min-w-max whitespace-nowrap text-xs text-muted-foreground/90">
					<span className="font-mono text-foreground">{buildSha.slice(0, 7)}</span>
					{buildTimeLabel ? (
						<>
							<span>{" / "}</span>
							<time className="font-mono" dateTime={buildTime}>
								{buildTimeLabel}
							</time>
						</>
					) : null}
				</p>
			</div>
		</footer>
	);
}
