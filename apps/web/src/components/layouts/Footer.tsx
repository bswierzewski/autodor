import dayjs from "dayjs";
import { formatDate } from "../../lib/formatters";

const buildInfo = import.meta.env as ImportMetaEnv & {
	readonly VITE_APP_GIT_SHA?: string;
	readonly VITE_APP_BUILD_TIME?: string;
};

const buildShaShort = buildInfo.VITE_APP_GIT_SHA?.trim()?.slice(0, 7) ?? "";
const buildTime = buildInfo.VITE_APP_BUILD_TIME?.trim() ?? "";
const buildTimeLabel =
	buildTime && dayjs(buildTime).isValid()
		? `${formatDate(buildTime)} ${dayjs(buildTime).format("HH:mm:ss")}`
		: "";
const currentYear = new Date().getFullYear();

export function Footer() {
	return (
		<footer className="border-t bg-background/95 px-4 py-4 text-sm text-muted-foreground lg:px-6">
			<div className="mx-auto flex w-full max-w-screen-2xl flex-wrap items-center justify-between gap-4">
				<p className="min-w-max whitespace-nowrap text-xs text-muted-foreground/90">
					<span>
						Copyright © {currentYear} <span className="font-medium text-foreground">bswierzewski</span>
					</span>
				</p>
				<p className="min-w-max whitespace-nowrap text-xs text-muted-foreground/90">
					<time className="font-mono" dateTime={buildTime || undefined}>
						{buildTimeLabel}
					</time>
					<span>{" | "}</span>
					<span className="font-mono">{buildShaShort}</span>
				</p>
			</div>
		</footer>
	);
}
