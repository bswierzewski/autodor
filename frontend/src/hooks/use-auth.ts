import { useUser } from "@clerk/react";

export enum UserStatus {
	Approved = "approved",
}

export type AuthContext = {
	isSignedIn: boolean;
	isApproved: boolean;
};

export function useAuth(): AuthContext {
	const { isSignedIn, user } = useUser();

	return {
		isSignedIn: Boolean(isSignedIn),
		isApproved: user?.publicMetadata?.status === UserStatus.Approved,
	};
}
