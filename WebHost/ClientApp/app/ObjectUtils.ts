export class ObjectUtils {
    public static isEmpty(data:any):boolean {
        if (typeof (data) === "number" || typeof (data) === "boolean" || data instanceof Date) {
            return false;
        }
        if (typeof (data) === "undefined" || data === null) {
            return true;
        }
        if (typeof (data.length) !== "undefined") {
            return data.length === 0;
        }

        for (var i in data) {
            if (data.hasOwnProperty(i)) {
                return false;
            }
        }
        return true;
    }

    public static isNotEmpty(data: any): boolean {
        return !ObjectUtils.isEmpty(data);
    }

    public static areEqual(a: any, b: any) {
        if(a === b) {
            return true;
        }

        if(typeof a === "number" && typeof b === "number") {
            if(Number.isNaN(a) && Number.isNaN(b)) {
                return true;
            }
        }

        return false;
    }

    public static areEqualByProperties(a: any, b: any) {
        if (ObjectUtils.areEqual(a, b)) {
            return true;
        }

        if (!a || !b || typeof a !== typeof b) {
            return false;
        }

        // date type comes here as object
        if (a.toString() !== "[object Object]" && a.toString() !== b.toString()) {
            return false;
        }

        for (var i in a) {
            if (a.hasOwnProperty(i) && (!b.hasOwnProperty(i) || !ObjectUtils.areEqualByProperties(a[i], b[i]))) {
                return false;
            }
        }
        return true;
    }

    public static GetBooleanFromString(value:string):boolean {
        return !!Number(value);
    }

    public static hasPropertySetter(value: any, propertyName: string): boolean {
        let proto = Object.getPrototypeOf(value);
        if (!proto) {
            return false;
        }

        let propertyDescriptor = Object.getOwnPropertyDescriptor(proto, propertyName);

        let hasSetter = ObjectUtils.isNotEmpty(propertyDescriptor) && ObjectUtils.isNotEmpty(propertyDescriptor.set);
        return hasSetter || ObjectUtils.hasPropertySetter(proto, propertyName);
    }
}