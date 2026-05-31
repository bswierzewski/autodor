import { ENV } from "../../config/env.ts";

const buildShaShort = ENV.VITE_APP_GIT_SHA.trim().slice(0, 7);
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
					<span className="font-mono">{buildShaShort}</span>
				</p>
			</div>
		</footer>
	);
}
