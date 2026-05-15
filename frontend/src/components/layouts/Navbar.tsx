import { UserButton, useUser } from "@clerk/react";
import { PackageIcon, UsersThreeIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";

const navItems = [
	{ label: "Zamówienia", to: "/", icon: PackageIcon },
	{ label: "Klienci", to: "/contractors", icon: UsersThreeIcon },
] as const;

export function Navbar() {
	const { user } = useUser();
	const firstName = user?.firstName ?? user?.fullName?.split(" ")[0] ?? "Użytkownik";
	const lastName = user?.lastName ?? user?.fullName?.split(" ").slice(1).join(" ") ?? "";

	return (
		<header className="flex items-center justify-between gap-6 rounded-b-2xl border px-5 py-4 shadow-sm backdrop-blur">
			<div className="shrink-0">
				<span className="text-lg font-black tracking-[0.2em]">AUTODOR</span>
			</div>
			<div className="flex items-center gap-4 sm:gap-6">
				<nav aria-label="Główna nawigacja" className="flex items-center gap-2 sm:gap-3">
					{navItems.map(({ label, to, icon: Icon }) => (
						<Link
							key={to}
							to={to}
							aria-label={label}
							className="flex items-center gap-2 rounded-full px-3 py-2 text-sm font-medium transition-colors"
							activeProps={{
								className: "flex items-center gap-2 rounded-full border px-3 py-2 text-sm font-medium shadow-sm",
							}}
						>
							<Icon size={18} weight="duotone" />
							<span className="hidden sm:inline">{label}</span>
						</Link>
					))}
				</nav>
				<div aria-hidden="true" className="h-10 w-px border-l" />
				<div className="flex items-center gap-3">
					<UserButton />
					<div className="hidden text-right sm:block">
						<div className="text-sm font-semibold leading-tight">{firstName}</div>
						{lastName ? <div className="text-sm leading-tight">{lastName}</div> : null}
					</div>
				</div>
			</div>
		</header>
	);
}
